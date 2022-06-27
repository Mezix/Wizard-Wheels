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
    [ReadOnly] public int _savedXSize = 0; //just the amount of Tiles in a given direction
    [ReadOnly] public int _savedYSize = 0;

    [ReadOnly] public int _tmpXSize = 0;
    [ReadOnly] public int _tmpYSize = 0;

    public XValues _savedMatrix = null;
    public XValues _tmpMatrix = null;
    public void InitTankForCreation()
    {
        //if (_savedMatrix.XArray.Length > 0)
        //{
        //    _savedXSize = _savedMatrix.XArray.Length;
        //    _savedYSize = _savedMatrix.XArray[0].YStuff.Length;
        //}
        ClearTmpMatrix();
        SavedToTmpMatrix();
        //CopyMatrixFromTo(_savedMatrix, _tmpMatrix);
        _tmpXSize = _savedXSize;
        _tmpYSize = _savedYSize;
    }
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
        TmpToSavedMatrix();
        //CopyMatrixFromTo(_tmpMatrix, _savedMatrix); //overwrite our old matrix with what is currently being shown
        _savedXSize = _tmpXSize;
        _savedYSize = _tmpYSize;

        #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        #endif
        Debug.Log("SAVED");
    }
    

    public void ClearTmpMatrix()
    {
        _tmpMatrix = new XValues(0, 0);
        _tmpXSize = 0;
        _tmpYSize = 0;
    }
    public void ClearSavedMatrix()
    {
        _savedMatrix = new XValues(0, 0);
        _savedXSize = 0;
        _savedYSize = 0;
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
        [HideInInspector] public Color FloorColor = Color.white;
        public Tile RoofTilePrefab = null;
        [HideInInspector] public Color RoofColor = Color.white;
        public GameObject SystemPrefab = null;
        public GameObject TirePrefab = null;

        public bool _topWallExists = false;
        public bool _rightWallExists = false;
        public bool _bottomWallExists = false;
        public bool _leftWallExists = false;
    }

    private void SavedToTmpMatrix()
    {
        if (_savedXSize == 0) _tmpMatrix = new XValues(0,0);
        else  _tmpMatrix = new XValues(_savedMatrix.XArray.Length, _savedMatrix.XArray[0].YStuff.Length);

        for (int x = 0; x < _savedMatrix.XArray.Length; x++)
        {
            for (int y = 0; y < _savedMatrix.XArray[x].YStuff.Length; y++)
            {
                _tmpMatrix.XArray[x].YStuff[y] = new RoomInfo();
                _tmpMatrix.XArray[x].YStuff[y].RoomPrefab = _savedMatrix.XArray[x].YStuff[y].RoomPrefab;
                _tmpMatrix.XArray[x].YStuff[y].RoofTilePrefab = _savedMatrix.XArray[x].YStuff[y].RoofTilePrefab;
                _tmpMatrix.XArray[x].YStuff[y].FloorTilePrefab = _savedMatrix.XArray[x].YStuff[y].FloorTilePrefab;
                _tmpMatrix.XArray[x].YStuff[y].SystemPrefab = _savedMatrix.XArray[x].YStuff[y].SystemPrefab;
                _tmpMatrix.XArray[x].YStuff[y].TirePrefab = _savedMatrix.XArray[x].YStuff[y].TirePrefab;
                _tmpMatrix.XArray[x].YStuff[y].SystemPrefab = _savedMatrix.XArray[x].YStuff[y].SystemPrefab;

                _tmpMatrix.XArray[x].YStuff[y]._topWallExists = _savedMatrix.XArray[x].YStuff[y]._topWallExists;
                _tmpMatrix.XArray[x].YStuff[y]._bottomWallExists = _savedMatrix.XArray[x].YStuff[y]._bottomWallExists;
                _tmpMatrix.XArray[x].YStuff[y]._leftWallExists = _savedMatrix.XArray[x].YStuff[y]._leftWallExists;
                _tmpMatrix.XArray[x].YStuff[y]._rightWallExists = _savedMatrix.XArray[x].YStuff[y]._rightWallExists;
                _tmpMatrix.XArray[x].YStuff[y].FloorColor = _savedMatrix.XArray[x].YStuff[y].FloorColor;
                _tmpMatrix.XArray[x].YStuff[y].RoofColor = _savedMatrix.XArray[x].YStuff[y].RoofColor;
            }
        }
    }
    private void TmpToSavedMatrix()
    {
        if (_tmpXSize == 0) _savedMatrix = new XValues(0, 0);
        _savedMatrix = new XValues(_tmpMatrix.XArray.Length, _tmpMatrix.XArray[0].YStuff.Length);

        for (int x = 0; x < _tmpMatrix.XArray.Length; x++)
        {
            for (int y = 0; y < _tmpMatrix.XArray[x].YStuff.Length; y++)
            {
                _savedMatrix.XArray[x].YStuff[y] = new RoomInfo();
                _savedMatrix.XArray[x].YStuff[y].RoomPrefab = _tmpMatrix.XArray[x].YStuff[y].RoomPrefab;
                _savedMatrix.XArray[x].YStuff[y].RoofTilePrefab = _tmpMatrix.XArray[x].YStuff[y].RoofTilePrefab;
                _savedMatrix.XArray[x].YStuff[y].FloorTilePrefab = _tmpMatrix.XArray[x].YStuff[y].FloorTilePrefab;
                _savedMatrix.XArray[x].YStuff[y].SystemPrefab = _tmpMatrix.XArray[x].YStuff[y].SystemPrefab;
                _savedMatrix.XArray[x].YStuff[y].TirePrefab = _tmpMatrix.XArray[x].YStuff[y].TirePrefab;
                _savedMatrix.XArray[x].YStuff[y].SystemPrefab = _tmpMatrix.XArray[x].YStuff[y].SystemPrefab;

                _savedMatrix.XArray[x].YStuff[y]._topWallExists = _tmpMatrix.XArray[x].YStuff[y]._topWallExists;
                _savedMatrix.XArray[x].YStuff[y]._bottomWallExists = _tmpMatrix.XArray[x].YStuff[y]._bottomWallExists;
                _savedMatrix.XArray[x].YStuff[y]._leftWallExists = _tmpMatrix.XArray[x].YStuff[y]._leftWallExists;
                _savedMatrix.XArray[x].YStuff[y]._rightWallExists = _tmpMatrix.XArray[x].YStuff[y]._rightWallExists;
                _savedMatrix.XArray[x].YStuff[y].FloorColor = _tmpMatrix.XArray[x].YStuff[y].FloorColor;
                _savedMatrix.XArray[x].YStuff[y].RoofColor = _tmpMatrix.XArray[x].YStuff[y].RoofColor;
            }
        }
    }
    private void CopyMatrixFromTo(XValues CopyFrom, XValues CopyTo)
    {
        if (_tmpXSize != _savedXSize || _tmpYSize != _savedYSize) CopyTo = new XValues(CopyFrom.XArray.Length, CopyFrom.XArray[0].YStuff.Length);

        for (int x = 0; x < CopyFrom.XArray.Length; x++)
        {
            for (int y = 0; y < CopyFrom.XArray[x].YStuff.Length; y++)
            {
                CopyTo.XArray[x].YStuff[y] = new RoomInfo();
                CopyTo.XArray[x].YStuff[y].RoomPrefab = CopyFrom.XArray[x].YStuff[y].RoomPrefab;
                CopyTo.XArray[x].YStuff[y].RoofTilePrefab = CopyFrom.XArray[x].YStuff[y].RoofTilePrefab;
                CopyTo.XArray[x].YStuff[y].FloorTilePrefab = CopyFrom.XArray[x].YStuff[y].FloorTilePrefab;
                CopyTo.XArray[x].YStuff[y].SystemPrefab = CopyFrom.XArray[x].YStuff[y].SystemPrefab;
                CopyTo.XArray[x].YStuff[y].TirePrefab = CopyFrom.XArray[x].YStuff[y].TirePrefab;
                CopyTo.XArray[x].YStuff[y].SystemPrefab = CopyFrom.XArray[x].YStuff[y].SystemPrefab;

                CopyTo.XArray[x].YStuff[y]._topWallExists = CopyFrom.XArray[x].YStuff[y]._topWallExists;
                CopyTo.XArray[x].YStuff[y]._bottomWallExists = CopyFrom.XArray[x].YStuff[y]._bottomWallExists;
                CopyTo.XArray[x].YStuff[y]._leftWallExists = CopyFrom.XArray[x].YStuff[y]._leftWallExists;
                CopyTo.XArray[x].YStuff[y]._rightWallExists = CopyFrom.XArray[x].YStuff[y]._rightWallExists;
            }
        }
    }
}