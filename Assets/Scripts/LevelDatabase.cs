using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDatabase
{
    public int scaleSize;
    public int BoardWidth;
    public int BoardHeight;
    public int BoyPos;
    public int GirlPos;
    public int NumberOfPieces;
    public int[] BoardEmptyTileTypeInfo;
    public PieceData[] pieceDatas;
    [HideInInspector]
    public bool tutorialCase = false;

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
        CheckNewPieces(data);
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
        switch (num)
        {
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

    string SetDefaultBoard()  //Return Default Board with all standard EmptyTiles
    {
        string board = "";
        for(int i = 0; i < BoardHeight*BoardWidth; i++)
        {
            board += "1";
        }
        return board;
    }

    void CheckNewPieces(int [] pieceinfo)//pieceinfo = stringinfo
    {
        int piecedata = PlayerPrefs.GetInt("Piecedata");//지금까지 타일
        int i = 0;
        for(;i< pieceinfo.Length;i++)
        {
            if (pieceinfo[i] > piecedata)//지금까지 나온 타일보다 크면 새로운 타일
            {
                tutorialCase = true;
                PlayerPrefs.SetInt("Piecedata", pieceinfo[i]);
            }
        }
    }
}
