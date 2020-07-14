using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Event : MonoBehaviour
{
    public AudioMixer audioMixer;   //오디오 믹서

    public Transform TileBoard;     //빈 타일(퍼즐판)의 Parent
    public Transform BlockPieces;   //아직 타일 위에 안 놓아진 퍼즐 조각들의 Parent
    public Transform BlockOnBoard;  //타일위에 놓아진 퍼즐 조각들의 Parent
    public GameObject Boy;          //파랭이
    public GameObject Girl;         //분홍이

    public Button ResetBtn;         //퍼즐 초기화 & 파랭이 움직임 리셋 버튼
    public Button GonfasterBtn;     //출발/가속 버튼

    //옵션 창
    public GameObject OptionMenu;
    public static bool GameIsPaused = false; // Game pause
    public Button OptionExit;       // 옵션 나가기

    //퍼즐 완료 창
    public GameObject PuzzleSolvedPanel;
    public Text coinText;
    [HideInInspector]
    public bool coinChangeToggle = true;

    Vector2 mousePos;               //마우스의 2차원상 위치
    Transform objToFollowMouse;     //마우스를 따라 다닐 물체(퍼즐 조각)
    GameObject[] triggeredObjects;  //Array stores info on EmptyTiles        //퍼즐 조각의 빈 타일 탐지용
    Vector3[] PiecePosition;        //Stores initial position of Pieces      //초기 퍼즐 조각 위치 저장

    float UIPieceScale = 0.4f;      //UI에서의 퍼즐 조각 크기. 화면/퍼즐에 놓았을 때는 1, UI상에서는 현재 값으로 축소

    int goNFastBtnState;            //1 -> Move Boy!, 2 -> Make Boy Faster!, 3 -> Make Boy back to normal speed     출발/가속 버튼용 변수

    //Variables for Level Loading
    LevelDatabase levelData;
    int levelNum;
    float scaleFactor;
    float distanceBetweenTiles;
    float emptyTileScale;
    float pieceScale;

    //boolean for Update Function //Update에 쓸 bool 변수
    [HideInInspector]
    public bool MovePieceMode;

    //튜토리얼
    int firstTime = 2;
    public GameObject tutorialPanel;
    bool tutorialDo = true;

    //세로용
    public static int ScreenWidth = Screen.width;
    public static int ScreenHeight = Screen.height;
    /*void Awake()
    {
        firstTime = PlayerPrefs.GetInt("tutorial");
    }*/

    void Start()
    {
        if(ScreenWidth == Screen.width && ScreenHeight == Screen.height)
        {
            ScreenHeight = Screen.height - (640 * 2);
            ScreenWidth = Screen.width - (360 * 2);
            Screen.SetResolution(ScreenWidth, ScreenHeight, true);
        }

        PlayerPrefs.SetInt("tutorial",0);//확인중임 없애도 됨. 기본은 0놓고했었음
        PlayerPrefs.SetInt("Piecedata", 1);
            stageLoad();
            //변수 초기화
            InitializeVariables();

            //levelData 게임 스테이지 데이터베이스에서 데이터를 불러와서 현재 스테이지 생성
            LoadLevel();

            //퍼즐 조각 초기 위치 저장
            SavePiecePosition();
    }

    void InitializeVariables()
    {
        MovePieceMode = true;

        goNFastBtnState = 1;
        GonfasterBtn.interactable = false;

        levelNum = 1;
        levelData = new LevelDatabase();

    }

    void Update()
    {
        //퍼즐조각 움직임 enable/disable
        if (MovePieceMode && Time.timeScale != 0f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //마우스 클릭시 충돌 물체 탐지
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    //클릭한 물체가 퍼즐 조각일 경우
                    if (hit.transform.tag == "Tile")
                    {
                        if(GonfasterBtn.interactable)
                        {
                            GonfasterBtn.interactable = false;
                        }

                        objToFollowMouse = hit.transform.parent;
                        objToFollowMouse.localScale = new Vector3(pieceScale, pieceScale, 1);//이건 아님

                        //Enable EmptyTile Box Collider2D & Detectors
                        for (int i = 0; i < objToFollowMouse.childCount; i++)
                        {
                            objToFollowMouse.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = 200;

                            //If Tile is on the board
                            if (objToFollowMouse.IsChildOf(BlockOnBoard))
                            {
                                objToFollowMouse.GetChild(i).GetChild(0).GetComponent<TileCollideDetection>().overlappedObject.GetComponent<BoxCollider2D>().enabled = true;  //Disable Box Collider of EmptyTile
                                objToFollowMouse.GetChild(i).GetChild(0).GetComponent<BoxCollider2D>().enabled = true;  //Enable Detector Box Collider
                            }
                        }

                        SoundFXPlayer.Play("pick");
                    }
                }
            }

            if (Input.GetMouseButton(0))
            {
                //마우스로 퍼즐 조각 드래그
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (objToFollowMouse != null)
                {
                    objToFollowMouse.position = new Vector3(mousePos.x, mousePos.y, 0);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (objToFollowMouse != null)
                {
                    bool isPiecePlaceable = true;
                    triggeredObjects = new GameObject[objToFollowMouse.childCount];

                    //Check if isPiecePlaceable
                    for (int i = objToFollowMouse.childCount - 1; i >= 0; i--)
                    {
                        if (objToFollowMouse.GetChild(i).GetChild(0).GetComponent<TileCollideDetection>().overlappedObject != null)
                        {
                            triggeredObjects[i] = objToFollowMouse.GetChild(i).GetChild(0).GetComponent<TileCollideDetection>().overlappedObject;
                            if (triggeredObjects[i].tag != "EmptyTile")
                            {
                                isPiecePlaceable = false;
                            }
                            if (!triggeredObjects[i].GetComponent<BoxCollider2D>().bounds.Contains(objToFollowMouse.GetChild(i).position))       //If the bounds of an EmptyTile doesn't contain the center point of the tile
                            {
                                isPiecePlaceable = false;
                            }
                        }
                        else
                        {
                            isPiecePlaceable = false;
                        }
                    }

                    if (isPiecePlaceable)
                    {
                        //Place piece on the board
                        objToFollowMouse.position = triggeredObjects[0].transform.position - (objToFollowMouse.GetChild(0).localPosition * pieceScale);//이부분인가?
                        objToFollowMouse.SetParent(BlockOnBoard, true);

                        //Disable Corresponding EmptyTile BoxCollider2D & Detectors
                        for (int i = 0; i < objToFollowMouse.childCount; i++)
                        {
                            triggeredObjects[i].GetComponent<BoxCollider2D>().enabled = false;
                            objToFollowMouse.GetChild(i).GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                            objToFollowMouse.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = 10;
                        }

                        SoundFXPlayer.Play("put");

                        CheckIfAllTilesInPlace();
                    }
                    else
                    {
                        ResetPiecePosition(objToFollowMouse, (Mathf.Abs(objToFollowMouse.localPosition.x) >= 5.0f && Mathf.Abs(objToFollowMouse.localPosition.x) < 8.5f && objToFollowMouse.localPosition.y >= -0.4f && objToFollowMouse.localPosition.y < 8.5f));
                    }

                    objToFollowMouse = null;
                }
            }
        }
    }

    //게임 레벨 불러오기
    void LoadLevel()
    {
        GameObject prefab;
        GameObject obj;
        GameObject obj2;

        levelData.LoadLevelData(levelNum);
        PlayerPrefs.SetInt("LevelDatabase", levelNum);//레벨 저장 일단
        int typeIndex;
        int pieceHeight;
        int pieceWidth;

        //여기다가 하는각
        if (levelData.tutorialCase && tutorialDo)// 바꾸어야될듯? -> leveldata에서 CheckNewPieces앞에 false해서 이제 ㄱㅊ
        {
            tutorialPanel.SetActive(true);
        }
        else
        {
            //Reset Position of BlockPieces
            BlockPieces.transform.position = new Vector3(-4f, -7f, 0);

            //Add Scaling by scaleSize!
            scaleFactor = 1f - 0.5f * (levelData.scaleSize - 1);// 모두에대한 scalersize 초기값이 0.5니까 이것보다는 커야된다
            distanceBetweenTiles = 0.75f * scaleFactor;
            emptyTileScale = 0.2f * scaleFactor;
            pieceScale = 0.75f * scaleFactor;
            /*Debug.Log("scaler : " + scaleFactor);
            Debug.Log("distance : " +distanceBetweenTiles);
            Debug.Log("empty : " + emptyTileScale);
            Debug.Log("piece : " + pieceScale);*/

            //Instantiate 'EmptyTile' - TileBoard에 해당하는것
            typeIndex = 0;
            for (int i = 0; i < levelData.BoardHeight; i++)
            {
                for (int j = 0; j < levelData.BoardWidth; j++)
                {
                    //Get prefab information from array                                                             //May Need for Optimization in the future
                    if (levelData.BoardEmptyTileTypeInfo[typeIndex] == 1)
                    {
                        prefab = Resources.Load("Prefabs/EmptyTile") as GameObject;
                    }
                    else if (levelData.BoardEmptyTileTypeInfo[typeIndex] == 0)
                    {
                        prefab = Resources.Load("Prefabs/VoidTile") as GameObject;
                    }
                    else
                    {
                        prefab = Resources.Load("Prefabs/FixedTile" + levelData.BoardEmptyTileTypeInfo[typeIndex].ToString()) as GameObject;
                    }

                    obj = Instantiate(prefab, new Vector3((-levelData.BoardWidth + 1) * (distanceBetweenTiles / 1f) + 2 * distanceBetweenTiles * j, (levelData.BoardHeight - 1) * (distanceBetweenTiles / 1f) - 2 * distanceBetweenTiles * i, 0), Quaternion.identity);//이부분이 생성하는 부분
                    obj.transform.localScale = new Vector3(emptyTileScale, emptyTileScale, 1);
                    obj.transform.SetParent(TileBoard, false);

                    //Set Boy & Girl Position
                    if (i == levelData.BoyPos && j == 0)
                    {
                        Boy.transform.position = obj.transform.position - new Vector3(0, 4*distanceBetweenTiles, 0);
                        Boy.GetComponent<MoveBoi>().initTargetPosition = obj.transform.position;
                        Boy.GetComponent<MoveBoi>().distanceBetweenTiles = distanceBetweenTiles;
                        Boy.transform.localScale = new Vector3(emptyTileScale, emptyTileScale, 1);
                    }

                    if (i == levelData.GirlPos && j == levelData.BoardWidth - 1)
                    {
                        Girl.transform.position = obj.transform.position + new Vector3(0, 4 * distanceBetweenTiles, 0);
                        Girl.transform.localScale = new Vector3(emptyTileScale, emptyTileScale, 1);
                    }
                    typeIndex++;
                }
            }

            //Random Puzzle Piece Position Version
            //PieceInitPosition = new Vector3[levelData.NumberOfPieces];
            for (int i = 0; i < levelData.NumberOfPieces; i++)
            {
                prefab = Resources.Load("Prefabs/Piece") as GameObject;
                obj = Instantiate(prefab, new Vector3(Random.Range(1.9f, 6.1f), Random.value < 0.5 ? Random.Range(3.5f, 5f) : Random.Range(9f, 10.5f)), Quaternion.identity);//z는 상관 없음 현재 x,y가 최소 최대값인데 이걸 좀 주려야된다. x : 1.6~6.4, y : 2.2 ~ 10f
                obj.transform.SetParent(BlockPieces, false);
                obj.GetComponent<VariableProvider>().pieceNum = i;

                typeIndex = 0;
                pieceHeight = levelData.pieceDatas[i].PieceHeight;
                pieceWidth = levelData.pieceDatas[i].PieceWidth;
                /*Debug.Log("pieceWidth : " + pieceWidth);
                Debug.Log("pieceHeight : " + pieceHeight);*/
                for (int j = 0; j < pieceHeight; j++)
                {
                    for (int k = 0; k < pieceWidth; k++)
                    {
                        if (levelData.pieceDatas[i].TileType[typeIndex] != 0)
                        {
                            prefab = Resources.Load("Prefabs/Tile" + levelData.pieceDatas[i].TileType[typeIndex].ToString()) as GameObject;
                            obj2 = Instantiate(prefab, new Vector3(-pieceWidth + 1 + 2 * k, pieceHeight - 1 - 2 * j, 0), Quaternion.identity);//이부분이 또 연산에 들어가고
                            obj2.transform.SetParent(obj.transform, false);
                            obj2.GetComponent<SpriteRenderer>().sortingOrder = 75 + i;
                        }
                        typeIndex++;
                    }
                }
            }
        }
        StartCoroutine(Waitsecond());
    }

    void SavePiecePosition()
    {
        PiecePosition = new Vector3[BlockPieces.childCount];
        for (int i = 0; i < BlockPieces.childCount; i++)
        {
            PiecePosition[i] = BlockPieces.GetChild(i).localPosition;
        }
    }

    //퍼즐 조각들이 모두 타일위에 놓아졌는지 확인
    void CheckIfAllTilesInPlace()
    {
        if(PiecePosition.Length == BlockOnBoard.childCount)
        {
            GonfasterBtn.interactable = true;
        }
        else
        {
            GonfasterBtn.interactable = false;
        }
    }

    //퍼즐 조각 하나의 위치 초기화
    void ResetPiecePosition(Transform piece, bool movePiecePosition = false)
    {
        piece.SetParent(BlockPieces);
        piece.localScale = new Vector3(UIPieceScale, UIPieceScale, 1);

        if (movePiecePosition)
        {
            PiecePosition[objToFollowMouse.GetComponent<VariableProvider>().pieceNum] = objToFollowMouse.localPosition;
        }
        else
        {
            piece.localPosition = PiecePosition[piece.GetComponent<VariableProvider>().pieceNum];
        }

        for (int i = 0; i < piece.childCount; i++)
        {
            piece.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = 75 + piece.GetComponent<VariableProvider>().pieceNum;
        }
    }

    //현재 스테이지 요소들 삭제
    void DeleteLevel()
    {
        for (int i = TileBoard.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(TileBoard.GetChild(i).gameObject);
        }

        for (int i = BlockPieces.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(BlockPieces.GetChild(i).gameObject);
        }

        for (int i = BlockOnBoard.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(BlockOnBoard.GetChild(i).gameObject);
        }
    }

    //보드위 BlockOnBoard 초기화
    void ResetBoard()
    {
        Transform objToReset;
        for (int i = BlockOnBoard.childCount - 1; i >= 0; i--)
        {
            objToReset = BlockOnBoard.GetChild(0);

            for (int j = 0; j < objToReset.childCount; j++)
            {
                objToReset.GetChild(j).GetChild(0).GetComponent<TileCollideDetection>().overlappedObject.GetComponent<BoxCollider2D>().enabled = true;
                objToReset.GetChild(j).GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
            }

            ResetPiecePosition(objToReset);
        }
    }

    //퍼즐 완료창 코인 또로로로 효과
    public IEnumerator CoinIncreaseAnimation(int coin = 100)
    {
        coinText.text = "";
        yield return new WaitForSeconds(0.5f);
        int i = 0;
        coinChangeToggle = true;
        while (i < coin + 1 && coinChangeToggle)
        {
            coinText.text = i.ToString();
            CoinFXPlayer.Play();
            i++;
            yield return null;
        }
    }

    /// <summary>
    /// 아래로 public 함수
    /// </summary>

    //출발/가속 버튼 State 1 -> 누르면 이동 시작, 2 -> 누르면 빨라짐, 3 -> 누르면 다시 원래 속도로 돌아옴
    public void GoNFastForwardClick()
    {
        if (goNFastBtnState == 1)
        {
            Boy.GetComponent<MoveBoi>().MoveDaBoi();
            MovePieceMode = false;
            goNFastBtnState = 2;
            GonfasterBtn.image.sprite = Resources.Load<Sprite>("Arts/FastForward");
            SoundFXPlayer.Play("go");
        }
        else if(goNFastBtnState == 2)
        {
            //Boy FastForward
            Boy.GetComponent<MoveBoi>().FastForward();
            goNFastBtnState = 3;
            GonfasterBtn.image.sprite = Resources.Load<Sprite>("Arts/NormalSpeed");
        }
        else
        {
            //Boy Back to normal speed
            Boy.GetComponent<MoveBoi>().BackToNormalSpeed();
            goNFastBtnState = 2;
            GonfasterBtn.image.sprite = Resources.Load<Sprite>("Arts/FastForward");
        }
    }

    //출발/가속 버튼 State 초기화
    public void ResetGoNFaster()
    {
        goNFastBtnState = 1;
        GonfasterBtn.image.sprite = Resources.Load<Sprite>("Arts/Goooo");
    }

    //초기화 버튼 눌렀을때
    public void ResetLevelClick()
    {
        if(Boy.GetComponent<MoveBoi>().isMoving) //During Boy Moving Phase
        {
            //Reset the boy moving
            Boy.GetComponent<MoveBoi>().ResetBoyMove();
            ResetGoNFaster();
        }
        else //During Puzzle Solving Phase
        {
            ResetBoard();
            GonfasterBtn.interactable = false;
        }
    }

    //다음 스테이지 불러오기
    public void GoToNextLevel()
    {
        levelNum++;
        MovePieceMode = true;
        ResetBtn.interactable = true;
        DeleteLevel();
        levelData.LoadLevelData(levelNum);//For check new tile;
        if (levelData.tutorialCase)// 바꾸어야될듯? -> leveldata에서 CheckNewPieces앞에 false해서 이제 ㄱㅊ
        {
            tutorialPanel.SetActive(true);
        }
        else
        {
            LoadLevel();
            SavePiecePosition();
        }

        //퍼즐 완료창 종료
        PuzzleSolvedPanel.SetActive(false);
        coinChangeToggle = false;
    }

    public void PlayAgain()
    {
        MovePieceMode = true;
        ResetBtn.interactable = true;
        ResetBoard();
        Boy.GetComponent<MoveBoi>().ResetBoyPosition();

        //퍼즐 완료창 종료
        PuzzleSolvedPanel.SetActive(false);
        coinChangeToggle = false;
    }

    //테스트용 개발자 버튼용
    public void DevBtnAct()  //Go To Level X
    {
        //바로 다음 퍼즐로 ㄱ
        levelNum++;
        DeleteLevel();
        LoadLevel();
        SavePiecePosition();
    }

    //옵션 버튼을 눌러 Option창 토글
    public void ToggleOptionPanel()
    {
        if(OptionMenu.activeSelf)
        {
            OptionMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            OptionMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    //옵션 창의 닫기 버튼을 눌러 Option창 닫기
    public void CloseOptionPanel()
    {
        OptionMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    //오디오믹서의 배경음악 볼륨 조절
    public void SetMusicVolume(float vol)
    {
        audioMixer.SetFloat("MusicVol", vol);
    }

    //오디오믹서의 효과음 볼륨 조절
    public void SetSFXVolume(float vol)
    {
        audioMixer.SetFloat("SFXVol", vol);
    }

    //오디오믹서의 환경음 볼륨 조절
    public void SetAmbienceVolume(float vol)
    {
        audioMixer.SetFloat("AmbienceVol", vol);
    }

    //save
    void stageSave()
    {
        PlayerPrefs.SetInt("tutorial", 1);
        PlayerPrefs.Save();
    }

    //load
    void stageLoad()
    {
        firstTime = PlayerPrefs.GetInt("tutorial");
    }

    //reset
    void stageReset()
    {
        PlayerPrefs.SetInt("tutorial", 1);
        PlayerPrefs.Save();
    }

    public void tutorialOff()//tutorial on/off -> 버튼
    {
        if (PlayerPrefs.GetInt("tutorial") == 0)// 0이면 처음부터
        {
            PlayerPrefs.SetInt("tutorial", 1);// 1이면 loadlevel 사용
            tutorialPanel.SetActive(false);
            InitializeVariables();
            LoadLevel();
            SavePiecePosition();
        }
        else if(PlayerPrefs.GetInt("tutorial") == 1 && tutorialDo)
        {
            tutorialPanel.SetActive(false);
            tutorialDo = false;
            //변수 초기화
            InitializeVariables();
            LoadLevel();
            SavePiecePosition();
        }
        else//level불러오는거
        {
            tutorialPanel.SetActive(false);
            LoadLevel();
            SavePiecePosition();
        }
    }
    IEnumerator Waitsecond()
    {
        yield return new WaitForSeconds(0.3f);
    }
}
