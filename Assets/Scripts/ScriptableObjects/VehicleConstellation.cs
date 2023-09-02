using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using System;
using UnityEngine.Tilemaps;
using static PlayerData;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/TankRoomConstellations")]
public class VehicleConstellation : ScriptableObject
{
    public int _savedXSize = 0; //just the amount of Tiles in a given direction
    public int _savedYSize = 0;

    public float FloorColorR = 1;
    public float FloorColorG = 1;
    public float FloorColorB = 1;
    public float RoofColorR  = 1;
    public float RoofColorG  = 1;
    public float RoofColorB  = 1;

    public XValues _savedMatrix = null;

    public void SaveVehicle(VehicleGeometry data)
    {
        CopyVehicleDataToTankRoomConstellation(data);

        #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        #endif
        Debug.Log("SAVED");
    }


    public void CopyVehicleDataToTankRoomConstellation(VehicleGeometry data)
    {
        _savedXSize = data.SavedXSize;
        _savedYSize = data.SavedYSize;

        RoofColorR = data.RoofColorR;
        RoofColorG = data.RoofColorG;
        RoofColorB = data.RoofColorB;

        FloorColorR = data.FloorColorR;
        FloorColorG = data.FloorColorG;
        FloorColorB = data.FloorColorB;

        _savedMatrix = new XValues(data.SavedXSize, data.SavedYSize);

        for (int x = 0; x < _savedXSize; x++)
        {
            for (int y = 0; y < _savedYSize; y++)
            {
                _savedMatrix.XArray[x].YStuff[y] = new RoomInfo();
                _savedMatrix.XArray[x].YStuff[y].RoomPrefabPath = data.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath;
                _savedMatrix.XArray[x].YStuff[y].FloorType = data.VehicleMatrix.Columns[x].ColumnContent[y].FloorType;
                _savedMatrix.XArray[x].YStuff[y].RoofType = data.VehicleMatrix.Columns[x].ColumnContent[y].RoofType;
                _savedMatrix.XArray[x].YStuff[y].SystemPrefabPath = data.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath;
                _savedMatrix.XArray[x].YStuff[y].MovementPrefabPath = data.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath;
                _savedMatrix.XArray[x].YStuff[y].SystemDirection = data.VehicleMatrix.Columns[x].ColumnContent[y].SystemDirection;

                _savedMatrix.XArray[x].YStuff[y]._topWallExists = data.VehicleMatrix.Columns[x].ColumnContent[y]._topWallExists;
                _savedMatrix.XArray[x].YStuff[y]._rightWallExists = data.VehicleMatrix.Columns[x].ColumnContent[y]._rightWallExists;
                _savedMatrix.XArray[x].YStuff[y]._bottomWallExists = data.VehicleMatrix.Columns[x].ColumnContent[y]._bottomWallExists;
                _savedMatrix.XArray[x].YStuff[y]._leftWallExists = data.VehicleMatrix.Columns[x].ColumnContent[y]._leftWallExists;
            }
        }
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
        public string RoomPrefabPath;
        public FloorType FloorType;
        public RoofType RoofType;
        public string SystemPrefabPath;
        public string MovementPrefabPath;
        public ASystem.DirectionToSpawnIn SystemDirection;

        public bool _topWallExists = false;
        public bool _rightWallExists = false;
        public bool _bottomWallExists = false;
        public bool _leftWallExists = false;
    }
    /*
    private void SavedToTmpMatrix()
    {
        CopyMatrixFromTo(ref _savedMatrix, ref _tmpMatrix);
    }
    private void TmpToSavedMatrix()
    {
        CopyMatrixFromTo(ref _tmpMatrix, ref _savedMatrix);
    }
    public static void CopyMatrixFromTo(ref XValues CopyFrom, ref XValues CopyTo)
    {
        CopyTo = new XValues(CopyFrom.XArray.Length, CopyFrom.XArray[0].YStuff.Length);

        for (int x = 0; x < CopyFrom.XArray.Length; x++)
        {
            for (int y = 0; y < CopyFrom.XArray[x].YStuff.Length; y++)
            {
                CopyTo.XArray[x].YStuff[y] = new RoomInfo();
                if (CopyFrom.XArray[x].YStuff[y] == null) continue;

                CopyTo.XArray[x].YStuff[y].RoomPrefabPath = CopyFrom.XArray[x].YStuff[y].RoomPrefabPath;
                CopyTo.XArray[x].YStuff[y].RoofType = CopyFrom.XArray[x].YStuff[y].RoofType;
                CopyTo.XArray[x].YStuff[y].FloorType = CopyFrom.XArray[x].YStuff[y].FloorType;
                CopyTo.XArray[x].YStuff[y].SystemPrefabPath = CopyFrom.XArray[x].YStuff[y].SystemPrefabPath;
                CopyTo.XArray[x].YStuff[y].SystemDirection = CopyFrom.XArray[x].YStuff[y].SystemDirection;
                CopyTo.XArray[x].YStuff[y].MovementPrefabPath = CopyFrom.XArray[x].YStuff[y].MovementPrefabPath;

                CopyTo.XArray[x].YStuff[y]._topWallExists = CopyFrom.XArray[x].YStuff[y]._topWallExists;
                CopyTo.XArray[x].YStuff[y]._bottomWallExists = CopyFrom.XArray[x].YStuff[y]._bottomWallExists;
                CopyTo.XArray[x].YStuff[y]._leftWallExists = CopyFrom.XArray[x].YStuff[y]._leftWallExists;
                CopyTo.XArray[x].YStuff[y]._rightWallExists = CopyFrom.XArray[x].YStuff[y]._rightWallExists;
            }
        }
    }
    */
}