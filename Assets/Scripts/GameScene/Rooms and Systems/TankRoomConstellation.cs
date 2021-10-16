using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(menuName = "ScriptableObjects/TankRoomConstellations")]
public class TankRoomConstellation : ScriptableObject
{
    public int XTilesAmount; //just the amount of Tiles in a given direction
    public int YTilesAmount;

    public RoomPosition[,] RoomPosMatrix;

    public XValues SavedPrefabRefMatrix;
    public XValues TmpPrefabRefMatrix;

    public void SaveConstellation()
    {
        SavedPrefabRefMatrix = TmpPrefabRefMatrix; //overwrite our old matrix with what is currently being shown
        Debug.Log("SAVED");
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
    public void InitialiseRoom() //intialises room with a new array3
    {
        RoomPosMatrix = new RoomPosition[XTilesAmount, YTilesAmount];
        TmpPrefabRefMatrix = new XValues(XTilesAmount, YTilesAmount);
        SavedPrefabRefMatrix = new XValues(XTilesAmount, YTilesAmount);
    }

    void CheckIfRoomInitialised() //simply checks if we havent initialised our room yet, and if so, creates it with the current xyz width
    {
        if (SavedPrefabRefMatrix.XArray.Length == 0)
        {
            InitialiseRoom();
        }
        if (!(RoomPosMatrix is object))
        {
            RoomPosMatrix = new RoomPosition[XTilesAmount, YTilesAmount];
        }
    }

    //WRAPPER CLASSES FOR SAVING STUFF

    [Serializable]
    public class XValues
    {
        public YValues[] XArray;
        public XValues(int xLength, int yLength)
        {
            XArray = new YValues[xLength];
            for (int i = 0; i < xLength; i++)
            {
                XArray[i] = new YValues(yLength);
            }
        }
    }
    [Serializable]
    public class YValues
    {
        public GameObject[] YRooms;
        public YValues(int yLength)
        {
            YRooms = new GameObject[yLength];
        }
    }
}