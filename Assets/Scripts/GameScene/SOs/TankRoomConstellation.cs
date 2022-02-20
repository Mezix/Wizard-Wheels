using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(menuName = "ScriptableObjects/TankRoomConstellations")]
public class TankRoomConstellation : ScriptableObject
{
    public int _X; //just the amount of Tiles in a given direction
    public int _Y;

    public int _tmpX;
    public int _tmpY;

    public XValues SavedPrefabRefMatrix;
    public XValues _tmpPrefabRefMatrix;

    public void SaveTank(string newName)
    {
        //change name
        if (newName != "")
        {
            string assetPath = AssetDatabase.GetAssetPath(GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, newName);
        }
        SavedPrefabRefMatrix = _tmpPrefabRefMatrix; //overwrite our old matrix with what is currently being shown
        _X = _tmpX;
        _Y = _tmpY;
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        Debug.Log("SAVED");
    }
    public void InitTankForCreation()
    {
        if(SavedPrefabRefMatrix.XArray.Length > 0)
        {
            _X = SavedPrefabRefMatrix.XArray.Length;
            _Y = SavedPrefabRefMatrix.XArray[0].YStuff.Length;
        }
        _tmpPrefabRefMatrix = SavedPrefabRefMatrix;
        _tmpX = _X;
        _tmpY = _Y;
    }

    public void ClearTank()
    {
        _tmpPrefabRefMatrix = new XValues(0,0);
        _tmpX = 0;
        _tmpY = 0;
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
        public GameObject Roof;
        public bool WallUp;
        public bool WallRight;
        public bool WallDown;
        public bool WallLeft;
    }
}