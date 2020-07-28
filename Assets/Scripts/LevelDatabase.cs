using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelDatabase
{
    public int scaleSize;
    public int BoardWidth;
    public int BoardHeight;
    public int BoyPos;
    public int GirlPos;
    public int NumberOfPieces;
    public int[] BoardEmptyTileTypeInfo;
    //public PieceData[] pieceDatas; //Temp -> Migrating from Array to List for dynamic allocation
    public List<PieceData> pieceDatas;

    [HideInInspector] public bool tutorialCase = false;
    [HideInInspector] public float dfac = 0;

    public class PieceData
    {
        public int PieceWidth;
        public int PieceHeight;
        //public int[] TileType; //Array -> List migration
        public List<int> TileType; //Array -> List migration
        public Vector3 solutionLoc; //solution for hint
    }

    class LoadedMapTile //need public?
    {
        public int LoadedTileCode;
        public bool isBoardSelected = false;
    }

    public int[] ConvertStringToIntArray(string data)
    {
        int[] arr = new int[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] < 58 && data[i] > 47)
            {
                arr[i] = data[i] - '0';
            }
            else if (data[i] > 64 && data[i] < 91)
            {
                arr[i] = data[i] - '7';
            }
            else
            {
                Debug.LogError("string to int[] Conversion Failed!! :(");
            }
        }
        return arr;
    }

    public void ConvertStringToPieceInfo(string s)
    {
        int[] data = ConvertStringToIntArray(s);
        tutorialCase = false;
        int d = 0;
        int pieceSize;
        //pieceDatas = new PieceData[NumberOfPieces]; //Array -> List migration
        pieceDatas = new List<PieceData>(); //Array -> List migration
        for (int i = 0; i < NumberOfPieces; i++) //piece index
        {
            //pieceDatas[i] = new PieceData(); //Array -> List migration
            pieceDatas.Add(new PieceData()); //Array -> List migration
            pieceSize = 1;

            pieceDatas[i].PieceWidth = data[d];
            pieceSize *= data[d];
            d++;
            pieceDatas[i].PieceHeight = data[d];
            pieceSize *= data[d];
            d++;
            //pieceDatas[i].TileType = new int[pieceSize]; //Array -> List migration
            pieceDatas[i].TileType = new List<int>();
            for (int j = 0; j < pieceSize; j++)
            {
                pieceDatas[i].TileType.Add(data[d]); //Array -> List migration
                //pieceDatas[i].TileType[j] = data[d]; //Array -> List migration
                d++;
            }
        }
    }

    public void LoadLevelData(int num)
    {
        switch (num)
        {
            case 0:
                break;
            case 1:
                scaleSize = 1;
                BoardWidth = 3;
                BoardHeight = 2;
                BoyPos = 0;
                GirlPos = 1;
                NumberOfPieces = 3;
                BoardEmptyTileTypeInfo = ConvertStringToIntArray(SetDefaultBoard());
                ConvertStringToPieceInfo("211421131241");
                break;
            case 2:
                scaleSize = 1;
                BoardWidth = 3;
                BoardHeight = 2;
                BoyPos = 0;
                GirlPos = 1;
                NumberOfPieces = 3;
                BoardEmptyTileTypeInfo = ConvertStringToIntArray(SetDefaultBoard());
                ConvertStringToPieceInfo("211821131291");
                break;
            case 3:
                scaleSize = 1;
                BoardWidth = 3;
                BoardHeight = 3;
                BoyPos = 0;
                GirlPos = 2;
                NumberOfPieces = 5;
                BoardEmptyTileTypeInfo = ConvertStringToIntArray(SetDefaultBoard());
                ConvertStringToPieceInfo("22301321141214113115");
                break;
            case 4:
                scaleSize = 2;
                BoardWidth = 4;
                BoardHeight = 4;
                BoyPos = 1;
                GirlPos = 2;
                NumberOfPieces = 7;
                BoardEmptyTileTypeInfo = ConvertStringToIntArray(SetDefaultBoard());
                ConvertStringToPieceInfo("1335122102112131234311522131114");
                break;
            case 5:
                scaleSize = 2;
                BoardWidth = 4;
                BoardHeight = 4;
                BoyPos = 0;
                GirlPos = 3;
                NumberOfPieces = 8;
                BoardEmptyTileTypeInfo = ConvertStringToIntArray(SetDefaultBoard());
                ConvertStringToPieceInfo("12572176117117226477127512461263");
                break;
            case 6:
                scaleSize = 3;
                BoardWidth = 5;
                BoardHeight = 5;
                BoyPos = 1;
                GirlPos = 3;
                NumberOfPieces = 10;
                BoardEmptyTileTypeInfo = ConvertStringToIntArray("1111111111116111111111111");
                ConvertStringToPieceInfo("13113224013220112221340215131531121311221141231");
                break;
        }
    }

    //txt파일에서 불러와서 string으로 return
    public string ReadFileByFactor(float dfactor)
    {
        TextAsset sourcefile = Resources.Load<TextAsset>(dfactor.ToString());
        StringReader sr = new StringReader(sourcefile.text);
        //string path = Application.persistentDataPath + "/Assets/Resources/" + dfactor + ".txt";
        //Debug.Log(Application.persistentDataPath);
        int random = Random.Range(1, 10000);
        #if UNITY_EDITOR
        bool existFolder = AssetDatabase.IsValidFolder("Assets/-1.txt");
        #endif
        if (existFolder)
        {
            Debug.Log("error");
            #if UNITY_EDITOR   
            string guid = AssetDatabase.CreateFolder("Assets", "My Folder");
            #endif
            random = -1;
        }
        else
        {
            random = Random.Range(1, 10000);
        }
        string testdata = "";
        for (int i = 0; i < random; i++)
        {
            testdata = sr.ReadLine();
        }
        Debug.Log("Read Line " + random);
        //Debug.Log(testdata);
        return testdata;
    }

    public void GenerateSlicedPieces(string s) //s : mapdata
    {
        /////////////////PROCESS MAP STRING INPUT//////////////////
        
        //Input string s Processing
        scaleSize = s[0] - '0';
        BoardWidth = s[1] - '0';
        BoardHeight = s[2] - '0';
        BoyPos = s[3] - '0';
        GirlPos = s[4] - '0';
        string BoardInput = s.Substring(5); //Board 입력받아오는값

        /////////////////PROCESS MAP STRING INPUT//////////////////

        /////////////////VARIABLES//////////////////
        //퍼즐판 크기 = 가로 길이 * 세로 길이
        int boardSize = BoardWidth * BoardHeight;

        //float difficultyFactor = dfac; // 난이도 조절용 -> (-1 ~ 1) -> 나중에 입력값으로 받음
        float difficultyFactor = Random.Range(-1f, 1f); //임시 Random 값

        //조각의 최대 크기(최대 타일 갯수) : boardSize = 4 -> 2, boardSize = 16 -> 4 // 수정 및 조절 필요
        int maxPieceSize = Mathf.FloorToInt(Mathf.Sqrt(boardSize));

        //생성할 조각 갯수 : BoardSize * (0.35 ~ 0.6) -> ex) boardSize = 16 -> (5.6 ~ 10.4)개, boardSize = 6 -> (2.1 ~ 3.9)개, difficultyFactor에 따라 범위 안에서 선택 // 수정 및 조절 필요
        NumberOfPieces = Mathf.Max(Mathf.RoundToInt(boardSize * (0.475f + (0.125f * difficultyFactor))), boardSize / maxPieceSize);

        //조각들 1차원 Array -> 조각 크기 할당용
        int[] pieceSizeArray = new int[NumberOfPieces];
        for (int i = 0; i < NumberOfPieces; i++) // Array 할당/초기화
        {
            pieceSizeArray[i] = 1;
        }
        /////////////////VARIABLES//////////////////

        ///TEST : Print Variables
        //Debug.Log("BoardSize : " + boardSize);
        //Debug.Log("MaxPieceSize : " + maxPieceSize);
        //Debug.Log("NumberOfPieces : " + NumberOfPieces);
        //Debug.Log("BoardInput : " + BoardInput);
        ///TEST : Print Variables

        ////////각 조각 갯수 할당 1번째 방법 ex) [boardSize : 9, NumberOfPieces : 5] -> [3,2,2,1,1]
        /*
        int remainingPieces = boardSize - NumberOfPieces; //boardSize piece중 할당하고 난 나머지
        for (int piecesize = 2; piecesize <= maxPieceSize; piecesize++)
        {
            int allocate = Random.Range(DivCeil(remainingPieces, maxPieceSize - (piecesize - 1)), Mathf.Min(remainingPieces, NumberOfPieces) + 1); //int의 '/'연산 잘 작동하는지 검증 필요, Random.Range difficultyFactor의 영향을 받도록 조정 필요
            for(int i = 0; i < allocate; i++)
            {
                pieceSizeArray[i] += 1;
            }
            remainingPieces -= allocate;
        }

        //유틸리티
        // int 두 개 나누고 소수는 올림 ex) [8 / 3 = 2.667 -> 3 return], [6 / 3 = 2 -> 2 return]
        int DivCeil(int a, int b) 
        {
            int c = a % b == 0 ? a / b : (a / b) + 1;
            return c;
        }
        */
        ////////각 조각 갯수 할당 1번째 방법

        ///////////각 조각 갯수 할당 2번째 방법/////////////
        // ex) [boardSize : 9, NumberOfPieces : 5] -> [2,2,1,3,1]
        int remainingPieces = boardSize - NumberOfPieces;
        int randomIndex;
        while(remainingPieces != 0) //[1,1,1,1,1]로 시작해서 Random으로 나오는 index의 값에 1씩 더함
        {
            randomIndex = Random.Range(0, NumberOfPieces);
            if(pieceSizeArray[randomIndex] != maxPieceSize) //maxPieceSize 넘어가는 것 방지
            {
                pieceSizeArray[randomIndex] += 1;
            }
            else
            {
                for(int i=0; i<NumberOfPieces; i++)
                {
                    if(pieceSizeArray[i] != maxPieceSize)
                    {
                        pieceSizeArray[i] += 1;
                        break;
                    }
                }
            }
            remainingPieces--;
        }
        ///////////각 조각 갯수 할당 2번째 방법/////////////

        ///TEST : Print pieceSizeArray
        /*
        for (int i = 0; i < pieceSizeArray.Length; i++)//들어가있는 갯수들이 pieceSizeArray = [2,2,1,3,1]꼴을 뛴다
        {
            Debug.Log("Piece " + i + ": " + pieceSizeArray[i] + " [pieceSizeArray]");
        }
        */
        ///TEST : Print pieceSizeArray

        //조각 잘라서 pieceDatas에 할당
        SliceBoard(BoardInput, pieceSizeArray);

        //Update Number of Pieces
        NumberOfPieces = pieceDatas.Count;

        //TEMP!!!!!! -> 나중에 EmptyTile 조정할 수 있는 방향으로 추가해야함(함수 새로 추가)
        BoardEmptyTileTypeInfo = ConvertStringToIntArray(SetDefaultBoard());
    }

    //조각 잘라서 pieceDatas에 할당
    void SliceBoard(string Board, int[] pieceSizeArray)
    {
        /////////////////VARIABLES//////////////////
        int remainingTiles = 0;
        int cur_X; //X position of current tile
        int cur_Y; //Y position of current tile
        int piece_start_X; //Top-Left [Y,X] Position of Piece
        int piece_start_Y;
        int piece_end_X; //Bottom-Right [Y,X] Position of Piece
        int piece_end_Y;
        List<Vector2Int> ValidTiles;
        List<Vector2Int> AddedTiles;
        int index; //범용성 Index
        bool noTilesLeftToAdd; //boolean handler for flow control when there is no ValidTile to add
        Vector2 boardCenter; //For Adding Location for Hints
        Vector2 pieceCenter; //For Adding Location for Hints
        /////////////////VARIABLES//////////////////

        /////////////////데이터 받아오기/////////////////
        LoadedMapTile[,] LoadedMap = new LoadedMapTile[BoardHeight, BoardWidth];
        index = 0; 
        //LoadedMap Array로 Input Map 정보 할당
        for (int i = 0; i < BoardHeight; i++) 
        {
            for (int j = 0; j < BoardWidth; j++)
            {
                LoadedMap[i, j] = new LoadedMapTile
                {
                    LoadedTileCode = Board[index] - '0'
                };
                index++;
            }
        }
        /////////////////데이터 받아오기/////////////////

        //////////////////////////////////////////////////////////////////////////////////조각 자르기//////////////////////////////////////////////////////////////////////////////////
        ///INIT///
        pieceDatas = new List<PieceData>();
        index = 0; //PieceSizeArray Iteration
        noTilesLeftToAdd = false; //The default value of noTilesLeftToAdd is false

        boardCenter = new Vector2((float)BoardHeight / 2, (float)BoardWidth / 2);
        ///INIT///

        //Board Iteration
        for (int i = 0; i < BoardHeight; i++)
        {
            for (int j = 0; j < BoardWidth; j++)
            {
                //여기가 할당되지 않은 타일일때
                if (!LoadedMap[i, j].isBoardSelected)
                {
                    //INIT as new piece start
                    pieceDatas.Add(new PieceData());
                    pieceDatas[pieceDatas.Count - 1].TileType = new List<int>(); // [pieceDatas.Count - 1] -> Last one Added
                    ValidTiles = new List<Vector2Int>();
                    AddedTiles = new List<Vector2Int>();
                    remainingTiles = pieceSizeArray[index];
                    noTilesLeftToAdd = false;

                    //PieceDatas & PieceData 할당
                    //Piece에 첫번째 시작 타일 할당
                    AddedTiles.Add(new Vector2Int(j, i));
                    cur_X = j;
                    cur_Y = i;
                    piece_start_X = j;
                    piece_start_Y = i;
                    piece_end_X = j;
                    piece_end_Y = i;
                    remainingTiles--;
                    LoadedMap[i, j].isBoardSelected = true;

                    while (remainingTiles != 0 && !noTilesLeftToAdd)
                    {
                        ///Check 동서남북 & Update ValidTiles
                        //Check North
                        if(cur_Y != 0) //Checking tile not out of bounds of Board
                        {
                            if(!LoadedMap[cur_Y - 1, cur_X].isBoardSelected) //Checking if north tile is already selected
                            {
                                ValidTiles.Add(new Vector2Int(cur_X, cur_Y - 1)); //Add North Tile to ValidTiles
                            }
                        }
                        //Check South
                        if(cur_Y + 1 != BoardHeight) //Checking tile not out of bounds of Board
                        {
                            if (!LoadedMap[cur_Y + 1, cur_X].isBoardSelected) //Checking if south tile is already selected
                            {
                                ValidTiles.Add(new Vector2Int(cur_X, cur_Y + 1)); //Add South Tile to ValidTiles
                            }
                        }
                        //Check West
                        if(cur_X != 0) //Checking tile not out of bounds of Board
                        {
                            if (!LoadedMap[cur_Y, cur_X - 1].isBoardSelected) //Checking if west tile is already selected
                            {
                                ValidTiles.Add(new Vector2Int(cur_X - 1, cur_Y)); //Add West Tile to ValidTiles
                            }
                        }
                        //Check East
                        if (cur_X + 1 != BoardWidth) //Checking tile not out of bounds of Board
                        {
                            if (!LoadedMap[cur_Y, cur_X + 1].isBoardSelected) //Checking if east tile is already selected
                            {
                                ValidTiles.Add(new Vector2Int(cur_X + 1, cur_Y)); //Add East Tile to ValidTiles
                            }
                        }

                        ///Select and Add a random Tile from Validtiles
                        //If there are no Tiles left to add
                        if(ValidTiles.Count == 0)
                        {
                            //End this piece
                            noTilesLeftToAdd = true;
                        }
                        else //Select a random tile from VaildTiles and add it to AddedTiles
                        {
                            int random = Random.Range(0, ValidTiles.Count);
                            int add_X = ValidTiles[random][0];
                            int add_Y = ValidTiles[random][1]; //순서바꿈

                            AddedTiles.Add(ValidTiles[random]);
                            LoadedMap[add_Y, add_X].isBoardSelected = true;

                            //Update cur_X, cur_Y
                            cur_X = ValidTiles[random][0];
                            cur_Y = ValidTiles[random][1]; //순서바꿈

                            //Update piece_start_X, piece_start_Y, piece_end_X, piece_end_Y, everytime a new tile is added from ValidTiles
                            if(add_X < piece_start_X)
                            {
                                piece_start_X = add_X;
                            }
                            if(add_Y < piece_start_Y)
                            {
                                piece_start_Y = add_Y;
                            }
                            if(add_X > piece_end_X)
                            {
                                piece_end_X = add_X;
                            }
                            if(add_Y > piece_end_Y)
                            {
                                piece_end_Y = add_Y;
                            }

                            ValidTiles.RemoveAt(random);
                            remainingTiles--;
                        }
                    }

                    //Convert AddedTiles -> PieceData.TileType format
                    pieceDatas[pieceDatas.Count - 1].PieceWidth = piece_end_X - piece_start_X + 1;
                    pieceDatas[pieceDatas.Count - 1].PieceHeight = piece_end_Y - piece_start_Y + 1;

                    //Iterate through the piece and sort out if tile exists or not
                    /*  [3]
                        [1][3] --> 3013  */
                    //[m,k]
                    for (int m = piece_start_Y; m < piece_end_Y + 1; m++)
                    {
                        for (int k = piece_start_X; k < piece_end_X + 1; k++)
                        {
                            if (ContainsTile(AddedTiles, m, k)) //Might be buggy
                            {
                                pieceDatas[pieceDatas.Count - 1].TileType.Add(LoadedMap[m, k].LoadedTileCode);
                            }
                            else
                            {
                                pieceDatas[pieceDatas.Count - 1].TileType.Add(0);
                            }
                        }
                    }

                    //Add Solution Location Vector for the Hint System
                    pieceCenter = new Vector2((float) (piece_end_X + piece_start_X) / 2 + 0.5f, (float) (piece_end_Y + piece_start_Y) / 2 + 0.5f); //(float) 실험
                    pieceDatas[pieceDatas.Count - 1].solutionLoc = pieceCenter - boardCenter;

                    //Update remainingTiles as next element from pieceSizeArray
                    if (!noTilesLeftToAdd) //If noTilesLeftToAdd = false
                    {
                        //Increase index, for remainingTiles update
                        index++;
                    }
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////////////조각 자르기//////////////////////////////////////////////////////////////////////////////////
    }

    string SetDefaultBoard() //Return Default Board with all standard EmptyTiles
    {
        string board = "";
        for(int i = 0; i < BoardHeight*BoardWidth; i++)
        {
            board += "1";
        }
        return board;
    }

    bool ContainsTile(List<Vector2Int> AddedTiles, int m, int k)
    {
        for(int i = 0; i < AddedTiles.Count; i++)
        {
            if(AddedTiles[i][1] == m && AddedTiles[i][0] == k)
            {
                return true;
            }
        }
        return false;
    }

    public void PieceCutterModuleTEST()
    {
        //TEST CASE 1
        /*
        BoardWidth = 3;
        BoardHeight = 2;
        SliceBoard("144131", new int[] { 2, 2, 2 });
        */

        //TEST CASE 2
        BoardWidth = 3;
        BoardHeight = 3;
        SliceBoard("141534313", new int[] { 2, 2, 1, 3, 1 });

        string TileTypeCode = "";
        for(int i = 0; i < pieceDatas.Count; i++)
        {
            Debug.Log("Piece " + (i + 1) + " : ");
            for(int j = 0; j < pieceDatas[i].TileType.Count; j++)
            {
                TileTypeCode = TileTypeCode + pieceDatas[i].TileType[j].ToString();
            }
            Debug.Log("PieceWidth : " + pieceDatas[i].PieceWidth + ", PieceHeight : " + pieceDatas[i].PieceHeight + ", TileTypeCode : " + TileTypeCode);
            TileTypeCode = "";
        }
    }
}
