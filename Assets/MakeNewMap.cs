using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MakeNewMap : MonoBehaviour
{
    [HideInInspector]
    public int scaleSize;
    [HideInInspector]
    public int BoardWidth;
    [HideInInspector]
    public int BoardHeight;
    [HideInInspector]
    public int BoyPos;
    [HideInInspector]
    public int GirlPos;
    [HideInInspector]
    public int NumberOfPieces;
    [HideInInspector]
    public int[] BoardEmptyTileTypeInfo;
    public PieceData[] pieceDatas;
    [HideInInspector]
    public string Newmap;
    public timestoper timer;
    [HideInInspector]
    public string nevergiveup;
    private moveboy boys;
    string check;
    int Girlposcheck = 10;
    int Boyposcheck = 10;
    int passivemap = 111222;
    int passivemapcheck = 0;
    int[,] passivemapmatrix;
    [HideInInspector]
    public float difficultyFactor = 0;

    public class PieceData
    {
        public int PieceWidth;
        public int PieceHeight;
        public int[] TileType;
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
        int d = 0;
        int pieceSize;
        pieceDatas = new PieceData[NumberOfPieces];
        for (int i = 0; i < NumberOfPieces; i++) //piece index
        {
            pieceDatas[i] = new PieceData();
            pieceSize = 1;

            pieceDatas[i].PieceWidth = data[d];
            pieceSize *= data[d];
            d++;
            pieceDatas[i].PieceHeight = data[d];
            pieceSize *= data[d];
            d++;
            pieceDatas[i].TileType = new int[pieceSize];
            for (int j = 0; j < pieceSize; j++)
            {
                pieceDatas[i].TileType[j] = data[d];
                d++;
            }
        }
    }

    public void makeNewlevel()
    {
        if(Newmap == null)//이부분 중요했음 ㅋㅋ 기존 맵 Destoryer
        {

        }
        else
        {
            Newmap = null;
        }
        BoardWidth = Random.Range(3, 5);
        BoardHeight = Random.Range(3, 5);
        int scalesizechanger = 0;
        scalesizechanger = (BoardWidth + BoardHeight) / 2;
        switch (BoardHeight)
        {
            case 1:
            case 2:
            case 3:
                scaleSize = 1;
                break;
            case 4:
                scaleSize = 2;
                break;
            case 5:
                scaleSize = 3;
                break;
        }
        BoyPos = Random.Range(0, BoardHeight - 1);
        GirlPos = Random.Range(0, BoardHeight - 1);
        int tilevalue = 0;
        int rangeoftile = 9;
        bool tile8yes = true;//tile8yes 없애면 tile9 1개랑 나머지는 tile8개로 나옴
        bool tile9yes = true;
        for (int i = 0; i < BoardHeight; i++)
            for (int j = 0; j < BoardWidth; j++)
            {
                tilevalue = Random.Range(1, rangeoftile);// 1부터
                if (tilevalue == 8 && tile8yes)
                {
                    if (tile9yes)
                    {
                        tilevalue = 9;
                        tile9yes = false;
                    }
                    else
                    {
                        tile8yes = false;
                        rangeoftile = 8;
                    }
                }
                //타일 Exception
                if (j == 0)//<-
                {
                    while (tilevalue == 2)
                    {
                        tilevalue = Random.Range(1, rangeoftile);
                    }
                }
                else if (i == 0)
                {
                    while (tilevalue == 5)
                    {
                        tilevalue = Random.Range(1, rangeoftile);
                    }
                }
                else if (j == BoardHeight - 1)
                {
                    while (tilevalue == 4)
                    {
                        tilevalue = Random.Range(1, rangeoftile);
                    }
                }
                else if (j == GirlPos && i == BoardWidth - 1)
                {
                    while (tilevalue == 2)
                    {
                        tilevalue = Random.Range(1, rangeoftile);
                    }
                }
                Newmap += tilevalue;
            }
        BoardEmptyTileTypeInfo = ConvertStringToIntArray(Newmap); // 이걸 통해 만들어서 확인후에 돌아가는지 확인하고 pieces 나누기로 합시당.
    }

    public void LoadLevelData(int num)
    {
        /*switch (num)
        {
            case 1:
            scaleSize = 1;
            BoardWidth = 3;
            BoardHeight = 2;
            BoyPos = 0;
            GirlPos = 0;
            NumberOfPieces = 0;
            Newmap = "333333";
            BoardEmptyTileTypeInfo = ConvertStringToIntArray(Newmap);
            break;
            case 2:
                makeNewlevel();
            break;
        }*/
        //makeNewlevel();
        passivemaker();

        //Mapmakingbydfactor(-1);
    }

    string SetDefaultBoard()  //Return Default Board with all standard EmptyTiles
    {
        string board = "";
        for (int i = 0; i < BoardHeight * BoardWidth; i++)
        {
            board += "1";
        }
        return board;
    }

    void Checkingsame()
    {
        if (check == Newmap)
        {
            if (GirlPos == Girlposcheck && BoyPos == Boyposcheck)
                LoadLevelData(0);
        }
        else
        {
            check = Newmap;
            Girlposcheck = GirlPos;
            Boyposcheck = BoyPos;
        }
    }

    void passivemaker()
    {
        float difficultyFactor = Random.Range(-1, 1);
        if(passivemap >= 10000)
            Application.Quit();
        if (Newmap == null)//이부분 중요했음 ㅋㅋ 기존 맵 Destoryer
        {

        }
        else
        {
            Newmap = null;
        }

        if (passivemapcheck == passivemap)
            passivemap++;

        BoardWidth = 2; // 2*2타일같이 여기서 수정함
        BoardHeight = 3;
        scaleSize = 1;
        passivemapmatrix = new int[BoardHeight, BoardWidth];
        //여자애 남자애 포지션
        BoyPos = 0;
        GirlPos = 1;

        //여기서 결정을 다 함.
        mapmatrix();

        //Debug.Log(passivemap);

        Newmap += passivemap;// 이 증가의 형태를 설명한다!
        /* 0 0  -> 0 0 -> 0 0 -> 0 1
           0 0     0 1    1 0    1 1 */

        passivemapcheck = passivemap;
        passivemap++;
        /* 다음에 할거
         Pin = 0000
print (Pin)
while Pin < 10000:
    print (Pin)
    Pin = Pin + 1
         */
        BoardEmptyTileTypeInfo = ConvertStringToIntArray(Newmap);
    }

    void mapmatrix()
    {
        //difficultyFactor가 -1~1 -> size스케일러
        int Big = 0;
        //int sizescaler = 0;
        //sizescaler = (passivemapmatrix.GetLength(0) + passivemapmatrix.GetLength(1));
        //ez case check

        //difficultyFactor는 각 파일에서 -1~1까지 하면된다.
        difficultyFactor = -1;

        if (BoardWidth > BoardHeight)
        {
            Big = BoardWidth;
        }
        else
        {
            Big = BoardHeight;
        }
        //3,2
        //111111
        passivemapmatrix[0, 0] = passivemap / 100000;
        passivemapmatrix[0, 1] = (passivemap / 10000) % 10;
        passivemapmatrix[1, 0] = (passivemap / 1000) % 10;
        passivemapmatrix[1, 1] = (passivemap / 100) % 10;
        passivemapmatrix[2, 0] = (passivemap / 10) % 10;
        passivemapmatrix[2, 1] = passivemap % 10;

        while(passivemapmatrix[BoyPos, 0] == 1 || passivemapmatrix[BoyPos, 0] == 2 || passivemapmatrix[BoyPos, 0] == 3 || passivemapmatrix[BoyPos, 0] == 5 || passivemapmatrix[BoyPos, 0] == 6)
            passivemapmatrix[BoyPos, 0] += 1;
        while(passivemapmatrix[GirlPos, 1] == 1 || passivemapmatrix[GirlPos, 1] == 2 || passivemapmatrix[GirlPos, 1] == 4 || passivemapmatrix[GirlPos, 1] == 5 || (passivemapmatrix[BoyPos, 0] == 8 && passivemapmatrix[GirlPos, 1] == 9) || (passivemapmatrix[BoyPos, 0] == 9 && passivemapmatrix[GirlPos, 1] == 8))// || passivemapmatrix[GirlPos, 1] == 6)
            passivemapmatrix[GirlPos, 1] += 1;
        /*if (passivemapmatrix[1, 0]== 4 || passivemapmatrix[1, 0] == 7)
            passivemapmatrix[1, 0] += 1;
        if (passivemapmatrix[1, 1] == 4 || passivemapmatrix[1, 1] == 7)
            passivemapmatrix[1, 1] += 1;*/

        /*Debug.Log("00:"+passivemapmatrix[0, 0]);
        Debug.Log("01:" + passivemapmatrix[0, 1]);
        Debug.Log("10:" + passivemapmatrix[1, 0]);
        Debug.Log("11:" + passivemapmatrix[1, 1]);*/

        int checkmatrixnum8 = 0;
        int checkmatrixnum9 = 0;
        //1천대
        if (passivemapmatrix[0, 0] == 0)
        {
            passivemapmatrix[0, 0] += 1;
        }
        else if(passivemapmatrix[0, 0] == 8)
        {
            checkmatrixnum8++;
        }
        else if (passivemapmatrix[0, 0] == 9)
        {
            checkmatrixnum9++;
        }

        //Debug.Log("00:" + passivemapmatrix[0, 0]);

        //1백대
        if (passivemapmatrix[0, 0] == 3 && passivemapmatrix[1, 1] == 2)
        {
            passivemapmatrix[1, 1] += 1;
        }

        if (passivemapmatrix[0, 1] == 0)
        {
            passivemapmatrix[0, 1] += 1;
        }
        else if (passivemapmatrix[0, 1] == 8)
        {
            checkmatrixnum8++;
        }
        else if (passivemapmatrix[0, 1] == 9)
        {
            checkmatrixnum9++;
        }

        //Debug.Log("01:" + passivemapmatrix[0, 1]);

        //10대
        if (passivemapmatrix[1, 0] == 0)
        {
            passivemapmatrix[1, 0] += 1;
        }
        else if (passivemapmatrix[1, 0] == 8)
        {
            checkmatrixnum8++;
        }
        else if (passivemapmatrix[1, 0] == 9)
        {
            checkmatrixnum9++;
        }

        //Debug.Log("10:" + passivemapmatrix[1, 0]);

        //1대
        if (passivemapmatrix[1, 0] == 3 && passivemapmatrix[1, 1] == 2)
        {
            passivemapmatrix[1, 1] += 1;
        }

        if (passivemapmatrix[1, 1] == 0)
        {
            passivemapmatrix[1, 1] += 1;
        }
        else if (passivemapmatrix[1, 1] == 8)
        {
            checkmatrixnum8++;
        }
        else if (passivemapmatrix[1, 1] == 9)
        {
            checkmatrixnum9++;
        }

        //Debug.Log("11:" + passivemapmatrix[1, 1]);

        if (passivemapmatrix[2, 0] == 0)
        {
            passivemapmatrix[2, 0] += 1;
        }
        else if (passivemapmatrix[2, 0] == 8)
        {
            checkmatrixnum8++;
        }
        else if (passivemapmatrix[2, 0] == 9)
        {
            checkmatrixnum9++;
        }

        if (passivemapmatrix[2, 0] == 3 && passivemapmatrix[2, 1] == 2)
        {
            passivemapmatrix[2, 1] += 1;
        }

        if (passivemapmatrix[2, 1] == 0)
        {
            passivemapmatrix[2, 1] += 1;
        }
        else if (passivemapmatrix[2, 1] == 8)
        {
            checkmatrixnum8++;
        }
        else if (passivemapmatrix[2, 1] == 9)
        {
            checkmatrixnum9++;
        }

        // 확인
        //Debug.Log("8:"+ checkmatrixnum8);
        //Debug.Log("9:" + checkmatrixnum9);
        if ((checkmatrixnum8 == 0 && checkmatrixnum9 == 0) || (checkmatrixnum8 == 1 && checkmatrixnum9==1))
        {
            //Debug.Log("ok");
        }
        else if(checkmatrixnum8 == 1 && checkmatrixnum9 == 0)
        {
            passivemap++;
            mapmatrix();
        }
        else
        {
            //Debug.Log("notok");
            passivemap++;
            mapmatrix();
        }

        passivemap = passivemapmatrix[0, 0] * 100000 + passivemapmatrix[0, 1] * 10000 + passivemapmatrix[1, 0] * 1000 + passivemapmatrix[1, 1] * 100 + passivemapmatrix[2, 0] * 10 + passivemapmatrix[2, 1];


        //difficulty factor 다 확인해야될듯?
        int k = 0; //타일 갯수 확인
        for (int i = 0; i < BoardHeight; i++)
        {
            for (int j = 0; j < BoardWidth; j++)
            {
                if (passivemapmatrix[i, j] > 1)
                    k++;
            }
        }
        // 타일 갯수랑 기본 타일 갯수랑 확인하기
        int DifTilenum = 0;
        DifTilenum = k - Big;
        if(DifTilenum < 0) // tile갯수가 너무 적음
        {
            passivemap++;
            difficultyFactor = -1;
            mapmatrix();
        }
        else if(DifTilenum == 0) // 최소난이도
        {
            difficultyFactor = -1;
        }
        else // 더 많은거니까
        {
            difficultyFactor = -1 + 0.01f * DifTilenum; // -0.5 , 0.0
        }

        int Tile2to5 = 0;
        int Tile6to7 = 0;
        int Tile8to9 = 0;
        for (int i = 0; i < BoardHeight; i++)
        {
            for (int j = 0; j < BoardWidth; j++)
            {
                if (passivemapmatrix[i, j] >=2 || passivemapmatrix[i, j] <= 5)
                {
                    Tile2to5++;
                }
                else if (passivemapmatrix[i, j] == 6 || passivemapmatrix[i, j] == 7)
                {
                    Tile6to7++;
                }
                else if (passivemapmatrix[i, j] == 8 || passivemapmatrix[i, j] == 9)
                {
                    Tile8to9++;
                }
            }
        }
        //2~5,6~7,8~9에 대한 부분 확인인데 여기서부터
        if(Tile8to9 == 2 || Tile8to9==0)
        {
            //ok
        }
        else
        {
            passivemap++;
            mapmatrix();
        }
        //
        if(DifTilenum == 0)
        {
            difficultyFactor = difficultyFactor + Tile8to9 * 0.03f + Tile6to7 * 0.04f;
        }
        else
        {
            difficultyFactor = difficultyFactor + Tile8to9 * 0.035f + Tile6to7 * 0.045f + Tile2to5 * 0.03f;
        }
        //연산을 해보면 2 *  x + 4*y = 0.2
        // x+y=1/2 2x = 0.08, 2y = 0.12
        if (difficultyFactor<-1)
        {
            difficultyFactor = -1;
        }
        else if(difficultyFactor>1)
        {
            difficultyFactor = 1;
        }
        /*
        if (난이도 >= 0.5)
        {
            워프 타일 X
            6,7번 타일 X
            공백 타일 갯수 = 난이도 * 0.5f
        }
        */
    }
}
