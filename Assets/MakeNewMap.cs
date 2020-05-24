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
    int passivemap = 8119;
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

        Mapmakingbydfactor(-1);
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

    void Mapmakingbydfactor(float dfactor)
    {
        float levelfactor = 0;
        var listoftile = new List<int>();

        if (dfactor<-1)
        {
            dfactor = -1;
        }
        else if(dfactor>1)
        {
            dfactor = 1;
        }
        //여기까지는 일단 초기 설정
        //2+2 = 4. -1~?, 2+3=5, 3+3=6, 4+4=8,5+5=10 즉 4~10 7단계 를 21로 나누니까 0.3기준으로 factor 형성
        int sum = 0;
        if(dfactor>=-1f || dfactor< -0.8f)//-1,-0.9
        {
            sum = 4;
            levelfactor = -1f;
        }
        else if(dfactor >= -0.8f || dfactor < -0.5f)//0.7,0.6,0.5
        {
            sum = 5;
            levelfactor = -0.8f;
        }
        else if (dfactor >= -0.5f || dfactor < -0.2f)//0.4,0.3,0.2
        {
            sum = 6;
            levelfactor = -0.5f;
        }
        else if (dfactor >= -0.2f || dfactor < 0.1f)//0.1,0.0,0.1
        {
            sum = 7;
            levelfactor = -0.2f;
        }
        else if (dfactor >= 0.1f || dfactor < 0.4f)//0.2,0.3,0.4
        {
            sum = 8;
            levelfactor = 0.1f;
        }
        else if (dfactor >= 0.4f || dfactor < 0.7f)//0.5,0.6,0.7
        {
            sum = 9;
            levelfactor = 0.4f;
        }
        else if (dfactor >= 0.7f || dfactor <= 1f)//0.8,0.9,1.0
        {
            sum = 10;
            levelfactor = 0.7f;
        }
        //sum 분리하기
        BoardWidth = Random.Range(2, sum);
        BoardHeight = sum - BoardWidth;
        passivemapmatrix = new int[BoardHeight, BoardWidth];//여기까지 dfactor에 따른 틀을 만드는 거고 내부를 어떻게할까...

        //Girlpos, Boypos
        BoyPos = Random.Range(0, BoardHeight - 1);
        GirlPos = Random.Range(0, BoardHeight - 1);

        //scalesizechanger
        switch (sum/2)
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

        int Big = 0;
        if(BoardWidth > BoardHeight)
        {
            Big = BoardWidth;
        }
        else
        {
            Big = BoardHeight;
        }


        //여기까지가 초기 설정값이다.

        /*2*2 -> boypos height 
        2*3 -> 차원축소해도 된다 -2*2
        3*3 -> 3*4이상의차원으로가니까 안되더라고 차원축소가 x*/
        //dfactor에 감소가 없는 것 -> row나 col중에서 큰 숫자만큼 타일이 있을 경우 1번인 공백타일 빼고
        //그 중에서 6,7번은 0.05의 난이도를 올리고, 8,9가 같이 있다면 0.03의 난이도를 올린다.
        //row =3인데 2~5의 타일이 4개라묜 난이도를 0.02를 올린다
        //반대로 8,9 타일이 오려면 levelfactor가 0.06남아있고 Big도 2개가 남아있어야만 나오는거면 별로자너
        //결국 추가가 먼저냐 아니면 대체가 먼저냐 이건데

        levelfactor = dfactor - levelfactor; // 남아있는 levelconstruction value
        int Tilerange = 5;
        if(levelfactor>=0.1)
        {
            Tilerange = 9;
        }
        //listoftil 만든다.
        //이걸 다시 배정하는것인데 흠
        //Random으로하지말고 Boy하고 Gril pos의 차이에 따라 우리가 줘야될것은 아래로 가는거랑 옆으로 가는거
        if(GirlPos>BoyPos)
        {
            listoftile.Add(3);
            listoftile.Add(5);
            Big = Big - 2;
        }
        else if(GirlPos < BoyPos)
        {
            listoftile.Add(3);
            listoftile.Add(4);
            Big = Big - 2;
        }
        else//같을때는 어떻게 할건데?
        {
            listoftile.Add(Random.Range(2, 5));
            listoftile.Add(Random.Range(2, 5));
            Big = Big - 2;
        }

        //그냥 levelfactor에 따라 나눠 버리는게 좋지 않냐?
        //마지막 factor빼고는 전부 0.3이하거든?
        //Big으로 남은 타일 갯수도 생각해야되는것인가 흠
        // levelfactor의 갯수들 8,9를 만들거면 최소 0.06필요
        // 6,7 대체의 경우는 0.05, 추가의경우는 1개당 0.07필요하다

        int k = Random.Range(0, 1);
        //levelfactor <=0.1 -> tile을 변경하는거지
        if (levelfactor <=0.1)//tile 교체
        {
            if(k==0)//6,7로 바꾸는거
            {
                int i = 0;
                for (; i < listoftile.Count; i++)
                {
                    listoftile[i] = Random.Range(2, 7);
                }
            }
            else//8,9로 바꾸는거
            {
                int i = 0;
                for (; i < listoftile.Count-2; i++)
                {
                    listoftile[i] = Random.Range(2, 7);
                }
                listoftile[i + 1] = 8;
                listoftile[i + 1] = 9;
            }
        }
        else if(levelfactor>0.1 && levelfactor<=0.2)//1개~(Boardh+w)*3/4-(Big+2) 추가가 적당할 거같음
        {
            if (k == 0)//6,7 추가 몇개를?
            {
                int i = 0;
                for (; i < listoftile.Count; i++)
                {
                    listoftile[i] = Random.Range(2, 7);
                }
            }
            else
            {
                int i = 0;
                for (; i < listoftile.Count - 2; i++)
                {
                    listoftile[i] = Random.Range(2, 7);
                }
                listoftile[i + 1] = 8;
                listoftile[i + 1] = 9;
            }
        }
        else
        {

        }

        k = 0;
        
        for(int i=0;i< BoardHeight;i++)
        {
            for (int j = 0; j < BoardWidth; j++)
            {
                passivemapmatrix[i, j] = listoftile[k];
                k++;
            }
        }
        listoftile.Clear();
    }

}
