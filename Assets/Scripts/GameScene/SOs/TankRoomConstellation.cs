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
    public int _XSize = 0; //just the amount of Tiles in a given direction
    public int _YSize = 0;

    public int _tmpXSize = 0;
    public int _tmpYSize = 0;

    public XValues _savedMatrix = null;
    public XValues _tmpMatrix = null;

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
        _savedMatrix = _tmpMatrix; //overwrite our old matrix with what is currently being shown
        _XSize = _tmpXSize;
        _YSize = _tmpYSize;

        #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        #endif
        Debug.Log("SAVED");
    }
    public void InitTankForCreation()
    {
        if(_savedMatrix.XArray.Length > 0)
        {
            _XSize = _savedMatrix.XArray.Length;
            _YSize = _savedMatrix.XArray[0].YStuff.Length;
        }
        _tmpMatrix = _savedMatrix;
        _tmpXSize = _XSize;
        _tmpYSize = _YSize;
    }

    public void ClearTank()
    {
        _tmpMatrix = new XValues(0,0);
        _tmpXSize = 0;
        _tmpYSize = 0;
    }

    //WRAPPER CLASSES FOR SAVING STUFF

    [Serializable]
    public class XValues
    {
        public YValues[] XArray = null;
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
        public RoomInfo[] YStuff = null;
        public YValues(int yLength)
        {
            YStuff = new RoomInfo[yLength];
        }
    }
    [Serializable]
    public class RoomInfo
    {
        //  Fields for loading and saving
        public GameObject RoomPrefab = null;
        public Tile FloorTilePrefab = null;
        public Tile RoofTilePrefab = null;
        public GameObject SystemPrefab = null;
        public GameObject TirePrefab = null;
        public bool _topWallExists = false;
        public bool _rightWallExists = false;
        public bool _bottomWallExists = false;
        public bool _leftWallExists = false;

        //spawned objects for referencing
    }
}