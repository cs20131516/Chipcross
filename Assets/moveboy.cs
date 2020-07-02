using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class moveboy : MonoBehaviour
{
    public Event_map eventmap;
    public timestoper timer;

    public GameObject TileBoard;
    public GameObject map;
    public Button GoToNextLevelBtn;
    public Button GoNFasterButton;
    public Button ResetButton;

    public float speed;
    public float distanceBetweenTiles;

    public Vector3 initTargetPosition;
    Vector3 targetPosition; // 이거 중요함....
    Vector3 boiInitPos;

    public bool isMoving;
    bool isThereNextTile;
    bool metGirl;

    float fastForwardFactor;

    char tileType;
    int xdir;
    int ydir;
    int temp;

    //Warp Tile용 변수
    bool warp = false;
    bool warpDone = false;

    //textfile용 변수

    void Start()
    {
        //isMoving = false;
        metGirl = false;
        GoToNextLevelBtn.interactable = false;
        //content = map.GetComponent<MakeNewMap>().nevergiveup;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * fastForwardFactor * Time.deltaTime);
            if (transform.position == targetPosition)
            {
                if(timer.timeLeft < 0)
                {
                    eventmap.ResetGoNFasterButton();
                    isMoving = false;
                    StartCoroutine(DelayBoyFail(1.5f));
                    Debug.Log("Try try again!");

                    eventmap.ChangeLevelAndMoveBoy();
                }
                if (isThereNextTile)     //파랭이가 아직 타일 위에 있는가?
                {
                    if (warp)            //Warp 해야 하는가?
                    {
                        if (tileType == '8')
                        {
                            if (GameObject.Find("Tile9(Clone)"))        //반대편 Warp 출구가 FixedTile 인지 Tile 인지 확인
                            {
                                gameObject.transform.position = GameObject.Find("Tile9(Clone)").transform.position;
                                StartCoroutine(Waitsecond());
                            }
                            else
                            {
                                gameObject.transform.position = GameObject.Find("FixedTile9(Clone)").transform.position;
                                StartCoroutine(Waitsecond());
                            }
                        }
                        else if (tileType == '9')
                        {
                            if (GameObject.Find("Tile8(Clone)"))       //반대편 Warp 출구가 FixedTile 인지 Tile 인지 확인
                            {
                                gameObject.transform.position = GameObject.Find("Tile8(Clone)").transform.position;
                                StartCoroutine(Waitsecond());
                            }
                            else
                            {
                                gameObject.transform.position = GameObject.Find("FixedTile8(Clone)").transform.position;
                                StartCoroutine(Waitsecond());
                            }
                        }
                        warp = false;
                        warpDone = true;
                    }
                    targetPosition = transform.position + new Vector3(xdir * distanceBetweenTiles, ydir * distanceBetweenTiles, 0);
                    isThereNextTile = false;
                }
                else                   //Finish moving; Reached Girl? Or try try again??
                {
                    eventmap.ResetGoNFasterButton();
                    isMoving = false;
                    if (metGirl) //Correct Solution
                    {
                        StartCoroutine(DelayBoyFail(1.5f));
                        Debug.Log("Puzzle solved!! Congrats!! :)");
                        metGirl = false;
                        CreateText(eventmap.checking);
                        DifficultyText(eventmap.difficultyfactor);
                        eventmap.ChangeLevelAndMoveBoy();
                        /*GoToNextLevelBtn.interactable = true;
                        GoNFasterButton.interactable = false;
                        ResetButton.interactable = false;
                        metGirl = false;*/
                    }
                    else //Wrong Solution
                    {
                        StartCoroutine(DelayBoyFail(1.5f));
                        Debug.Log("Try try again!");

                        eventmap.ChangeLevelAndMoveBoy();
                    }
                }
            }
        }
    }

    public void MoveDaBoi()
    {
        boiInitPos = transform.position;
        fastForwardFactor = 3f;
        xdir = 1;
        ydir = 0;
        targetPosition = initTargetPosition;
        GetComponent<BoxCollider2D>().enabled = true;
        isMoving = true;
    }

    public void FastForward()
    {
        fastForwardFactor = 5f;
    }

    public void BackToNormalSpeed()
    {
        fastForwardFactor = 1f;
    }

    public void ResetBoyMove()
    {
        //Make the boy stop moving and return to its original position
        eventmap.ResetGoNFasterButton();
        eventmap.MovePieceMode = true;
        transform.position = boiInitPos;
        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Tile")
        {
            if (warpDone)           //워프에서 반대편으로 나왔을때 바로 타일 탐지하는것 방지
            {
                warpDone = false;
            }
            else
            {
                isThereNextTile = true;
                tileType = collision.gameObject.name[4];

                switch (tileType)
                {
                    case '1':
                        break;
                    case '2':
                        xdir = -1;
                        ydir = 0;
                        break;
                    case '3':
                        xdir = 1;
                        ydir = 0;
                        break;
                    case '4':
                        xdir = 0;
                        ydir = -1;
                        break;
                    case '5':
                        xdir = 0;
                        ydir = 1;
                        break;
                    case '6':
                        temp = xdir;
                        xdir = -ydir;
                        ydir = temp;
                        break;
                    case '7':
                        temp = xdir;
                        xdir = ydir;
                        ydir = -temp;
                        break;
                    case '8':
                        /*
                        gameObject.transform.position = GameObject.Find("Tile9(Clone)").transform.position;
                        targetPosition = GameObject.Find("Tile9(Clone)").transform.position;
                        warpDone = true;
                        */
                        warp = true;
                        break;
                    case '9':
                        /*
                        gameObject.transform.position = GameObject.Find("Tile8(Clone)").transform.position;
                        targetPosition = GameObject.Find("Tile8(Clone)").transform.position;
                        warpDone = true;
                        */
                        warp = true;
                        break;
                }
            }

            /*if (warpDone && (tileType == '8' || tileType == '9'))
            {
                tileType = '1';
                warpDone = false;
            }*/

        }
        else if (collision.tag == "FixedTile")
        {
            if (warpDone)           //워프에서 반대편으로 나왔을때 바로 타일 탐지하는것 방지
            {
                warpDone = false;
            }
            else
            {
                isThereNextTile = true;
                tileType = collision.gameObject.name[9];

                switch (tileType)
                {
                    case '1':
                        break;
                    case '2':
                        xdir = -1;
                        ydir = 0;
                        break;
                    case '3':
                        xdir = 1;
                        ydir = 0;
                        break;
                    case '4':
                        xdir = 0;
                        ydir = -1;
                        break;
                    case '5':
                        xdir = 0;
                        ydir = 1;
                        break;
                    case '6':
                        temp = xdir;
                        xdir = -ydir;
                        ydir = temp;
                        break;
                    case '7':
                        temp = xdir;
                        xdir = ydir;
                        ydir = -temp;
                        break;
                    case '8':
                        /*
                        gameObject.transform.position = GameObject.Find("Tile9(Clone)").transform.position;
                        targetPosition = GameObject.Find("FixedTile9(Clone)").transform.position;
                        warpDone = true;
                        */
                        warp = true;
                        break;
                    case '9':
                        /*
                        gameObject.transform.position = GameObject.Find("Tile8(Clone)").transform.position;
                        targetPosition = GameObject.Find("FixedTile8(Clone)").transform.position;
                        warpDone = true;
                        */
                        warp = true;
                        break;
                }
            }

            /*if (warpDone && (tileType == '8' || tileType == '9'))
            {
                tileType = '1';
                warpDone = false;
            }*/

        }
        else if (collision.gameObject.name == "Girl")
        {
            xdir = 0;
            ydir = 0;
            metGirl = true;
        }
    }

    IEnumerator DelayBoyFail(float time)
    {
        yield return new WaitForSeconds(time);
        eventmap.MovePieceMode = true;
        //transform.position = boiInitPos;
    }

    public void CreateText(string content)
    {
        //Path of the file
        string path = Application.dataPath + "/Mapcontent.txt";
        //Create File if it doesn't exist
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "start \n\n");
        }
        //Content of the file
        //Add some to text to it
        File.AppendAllText(path, "12300" + content + "\n");
    }
    public void DifficultyText(float content)
    {
        //Path of the file
        string path = Application.dataPath + "/LevelDifficulty.txt";
        //Create File if it doesn't exist
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "start \n\n");
        }
        //Content of the file
        //Add some to text to it
        File.AppendAllText(path,content + "\n");
    }
    public IEnumerator Waitsecond()
    {
        speed = 0f;
        yield return new WaitForSeconds(0.3f);
        speed = 3f;
    }
}
