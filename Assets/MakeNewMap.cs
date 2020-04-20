﻿using System.Collections;
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
        BoardWidth = Random.Range(3, 5);
        BoardHeight = Random.Range(3, 5);
        int scalesizechanger = 0;
        scalesizechanger = (BoardWidth + BoardHeight) / 2;
        switch(BoardHeight)
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
        BoyPos = Random.Range(0, BoardHeight-1);
        GirlPos = Random.Range(0, BoardHeight-1);
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
        makeNewlevel();
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
}
