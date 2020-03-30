using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

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

    public GameObject OptionMenu;   //옵션 창
    public static bool GameIsPaused = false; // Game pause
    public Button OptionExit;       // 옵션 나가기

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
    public bool MovePieceMode;

    int hohoho; //개발자 버튼용 변수

    //튜토리얼
    int firstTime = 1;
    public GameObject tutorialPanel;
    public GameObject Tile;

    /*void Awake()
    {
        firstTime = PlayerPrefs.GetInt("tutorial");
    }*/

    void Start()
    {
        if (firstTime == 1)// 튜토리얼 시작
        {
            //PlayerPrefs.SetInt("tutorial", 0);
            PlayerPrefs.SetInt("Piecedata", 5);
            //PlayerPrefs.Save();
            tutorialPanel.SetActive(true);
        }
        else// load
        {
            stageLoad();
            //변수 초기화
            InitializeVariables();

            //levelData 게임 스테이지 데이터베이스에서 데이터를 불러와서 현재 스테이지 생성
            LoadLevel();

            //퍼즐 조각 초기 위치 저장
            SavePiecePosition();

            //개발자 버튼용
            hohoho = 1;
        }
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
                        objToFollowMouse.localScale = new Vector3(pieceScale, pieceScale, 1);

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
                        objToFollowMouse.position = triggeredObjects[0].transform.position - (objToFollowMouse.GetChild(0).localPosition * scaleFactor);
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
        //여기서 판별을 해야되나?

        int typeIndex;
        int pieceHeight;
        int pieceWidth;

        //Reset Position of BlockPieces
        BlockPieces.transform.position = new Vector3(0, -3.75f, 0);

        //Add Scaling by scaleSize!
        scaleFactor = 1 - 0.2f * (levelData.scaleSize - 1);
        distanceBetweenTiles = 2 * scaleFactor;
        emptyTileScale = 0.25f * scaleFactor;
        pieceScale = 1 * scaleFactor;

        //Instantiate 'EmptyTile'
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

                obj = Instantiate(prefab, new Vector3((-levelData.BoardWidth + 1) * (distanceBetweenTiles / 2f) + distanceBetweenTiles * j, (levelData.BoardHeight - 1) * (distanceBetweenTiles / 2f) - distanceBetweenTiles * i, 0), Quaternion.identity);
                obj.transform.localScale = new Vector3(emptyTileScale, emptyTileScale, 1);
                obj.transform.SetParent(TileBoard, false);

                //Set Boy & Girl Position
                if (i == levelData.BoyPos && j == 0)
                {
                    Boy.transform.position = obj.transform.position - new Vector3(distanceBetweenTiles, 0, 0);
                    Boy.GetComponent<MoveBoi>().initTargetPosition = obj.transform.position;
                    Boy.GetComponent<MoveBoi>().distanceBetweenTiles = distanceBetweenTiles;
                    Boy.transform.localScale = new Vector3(emptyTileScale, emptyTileScale, 1);
                }

                if(i == levelData.GirlPos && j == levelData.BoardWidth - 1)
                {
                    Girl.transform.position = obj.transform.position + new Vector3(distanceBetweenTiles, 0, 0);
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
            obj = Instantiate(prefab, new Vector3(Random.value < 0.5 ? Random.Range(-7.6f, -5.9f) : Random.Range(5.9f, 7.6f), Random.Range(0, 7.4f)), Quaternion.identity);
            obj.transform.SetParent(BlockPieces, false);
            obj.GetComponent<VariableProvider>().pieceNum = i;

            typeIndex = 0;
            pieceHeight = levelData.pieceDatas[i].PieceHeight;
            pieceWidth = levelData.pieceDatas[i].PieceWidth;
            for (int j = 0; j < pieceHeight; j++)
            {
                for (int k = 0; k < pieceWidth; k++)
                {
                    if (levelData.pieceDatas[i].TileType[typeIndex] != 0)
                    {
                        prefab = Resources.Load("Prefabs/Tile" + levelData.pieceDatas[i].TileType[typeIndex].ToString()) as GameObject;
                        obj2 = Instantiate(prefab, new Vector3(-pieceWidth + 1 + 2 * k, pieceHeight - 1 - 2 * j, 0), Quaternion.identity);
                        obj2.transform.SetParent(obj.transform, false);
                        obj2.GetComponent<SpriteRenderer>().sortingOrder = 75 + i;
                    }
                    typeIndex++;
                }
            }
        }
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
        foreach (Transform child in TileBoard)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in BlockPieces)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in BlockOnBoard)
        {
            Destroy(child.gameObject);
        }
    }

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
            //Reset Position of BlockPieces
            BlockPieces.transform.position = new Vector3(0, -3.75f, 0);

            Transform objToReset;
            while (BlockOnBoard.childCount > 0)
            {
                objToReset = BlockOnBoard.GetChild(0);

                for (int i = 0; i < objToReset.childCount; i++)
                {
                    objToReset.GetChild(i).GetChild(0).GetComponent<TileCollideDetection>().overlappedObject.GetComponent<BoxCollider2D>().enabled = true;
                    objToReset.GetChild(i).GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
                }

                ResetPiecePosition(BlockOnBoard.GetChild(0));
            }

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
        if(levelData.tutorialCase)
        {
            Tile.GetComponent<Image>().sprite = Resources.Load<Sprite>("Arts/Theme1Tile" + PlayerPrefs.GetInt("Piecedata"));
            tutorialPanel.SetActive(true);
        }
        else
        {
            LoadLevel();
            SavePiecePosition();
        }
    }

    //테스트용 개발자 버튼용
    public void DevBtnAct()  //Go To Level X
    {
        /*levelNum++;
        DeleteLevel();
        LoadLevel();
        Debug.Log("BlockPieces.childCount : " + BlockPieces.childCount);
        SavePieceInitPosition();
        for(int i = 0; i < PieceInitPosition.Length; i++)
        {
            Debug.Log(" i = " + i + " : " + PieceInitPosition[i]);
        }
        Debug.Log("BlockPieces.childCount : " + BlockPieces.childCount);*/

        //Change to next Level
        if (hohoho == 1)
        {
            DeleteLevel();
            //Debug.Log("BlockPieces.childCount : " + BlockPieces.childCount);
            hohoho++;
        }
        else
        {
            levelNum++;
            LoadLevel();
            SavePiecePosition();
            //Debug.Log("BlockPieces.childCount : " + BlockPieces.childCount);
            hohoho = 1;
        }
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

    public void tutorialOff()
    {
        //Debug.Log(PlayerPrefs.GetInt("Piecedata"));
        if(PlayerPrefs.GetInt("Piecedata")==5)
        {
            tutorialPanel.SetActive(false);
            //변수 초기화
            InitializeVariables();

            //levelData 게임 스테이지 데이터베이스에서 데이터를 불러와서 현재 스테이지 생성
            LoadLevel();

            //퍼즐 조각 초기 위치 저장
            SavePiecePosition();

            //개발자 버튼용
            hohoho = 1;
        }
        else
        {
            tutorialPanel.SetActive(false);
            LoadLevel();
            SavePiecePosition();
        }
    }
}
