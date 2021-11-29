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

    //public RoomPosition[,] RoomPosMatrix;

    public XValues SavedPrefabRefMatrix;
    public XValues TmpPrefabRefMatrix;

    public void SaveConstellation()
    {
        SavedPrefabRefMatrix = TmpPrefabRefMatrix; //overwrite our old matrix with what is currently being shown
        Debug.Log("SAVED");
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
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
        public RoomInfo[] YStuff;
        public YValues(int yLength)
        {
            YStuff = new RoomInfo[yLength];
        }
    }
    [Serializable]
    public class RoomInfo
    {
        public GameObject RoomPrefab;
        public GameObject RoomSystemPrefab;
        public GameObject TirePrefab;
        public RoomInfo(GameObject prefab = null, GameObject sys = null)
        {
            RoomPrefab = prefab;
            RoomSystemPrefab = sys;
        }
    }
}