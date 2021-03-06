using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using System;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "ScriptableObjects/TankRoomConstellations")]
public class TankRoomConstellation : ScriptableObject
{
    public int _X; //just the amount of Tiles in a given direction
    public int _Y;

    public int _tmpX;
    public int _tmpY;

    public XValues SavedPrefabRefMatrix;
    public XValues _tmpMatrix;

    public void SaveTank(string newName)
    {
        //change name
        if (newName != "")
        {
            #if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, newName);
            #endif
        }
        SavedPrefabRefMatrix = _tmpMatrix; //overwrite our old matrix with what is currently being shown
        _X = _tmpX;
        _Y = _tmpY;

        #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        #endif
        Debug.Log("SAVED");
    }
    public void InitTankForCreation()
    {
        if(SavedPrefabRefMatrix.XArray.Length > 0)
        {
            _X = SavedPrefabRefMatrix.XArray.Length;
            _Y = SavedPrefabRefMatrix.XArray[0].YStuff.Length;
        }
        _tmpMatrix = SavedPrefabRefMatrix;
        _tmpX = _X;
        _tmpY = _Y;
    }

    public void ClearTank()
    {
        _tmpMatrix = new XValues(0,0);
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
        public Tile FloorTile;
        public GameObject RoomSystemPrefab;
        public GameObject TirePrefab;
        public Tile RoofTile;
        public bool WallUp;
        public bool WallRight;
        public bool WallDown;
        public bool WallLeft;
    }
}