using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerData;

public class DataStorage : MonoBehaviour
{
    public static DataStorage Singleton { get; private set; }

    public PlayerData playerData; // used to save at the end of an event, after certain "completion stages"
    public int saveSlot;
    public void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
            playerData = SavePlayerData.LoadPlayer(saveSlot);
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Update()
    {
        if (playerData != null) playerData.TimeInSecondsPlayed += Time.deltaTime;
    }

    public void AddInventoryItemToPlayerData(InventoryItemData item)
    {
        int index = playerData.CheckIfInventoryContains(item);
        if (index != -1)
        {
            playerData.InventoryList[index] = new InventoryItemData
            {
                Name = playerData.InventoryList[index].Name,
                SpritePath = playerData.InventoryList[index].SpritePath,
                Amount = item.Amount
            };
        }
        else
        {
            playerData.InventoryList.Add(item);
        }
    }
    public void AddInventoryItemsToPlayerData(List<InventoryItemData> items)
    {
        foreach (InventoryItemData itemToAdd in items)
        {
            int index = playerData.CheckIfInventoryContains(itemToAdd);
            if (index != -1)
            {
                playerData.InventoryList[index] = new InventoryItemData
                {
                    Name = playerData.InventoryList[index].Name,
                    SpritePath = playerData.InventoryList[index].SpritePath,
                    Amount = playerData.InventoryList[index].Amount + itemToAdd.Amount
                };
            }
            else
            {
                playerData.InventoryList.Add(itemToAdd);
            }
        }
    }


    public void AddWizard(WizardData wiz)
    {
        if (playerData.WizardList == null) playerData.WizardList = new List<WizardData>();
        playerData.WizardList.Add(wiz);
    }
    public void FinishEvent()
    {
        playerData.CurrentEventPath[playerData.CurrentEventPathIndex] = new EventNode(playerData.CurrentEventPath[playerData.CurrentEventPathIndex]._event, true);
        playerData.CurrentEventPathIndex++;
        SavePlayerData.SavePlayer(saveSlot, playerData);

        if (playerData.CurrentEventPathIndex >= playerData.CurrentEventPath.Count) Loader.Load(Loader.SceneType.VictoryScene);
        else Loader.Load(Loader.SceneType.RouteTransitionScene);
    }

    public void AdjustWizardHappiness(int wizIndex, int happinessAdjustmentAmount)
    {
        playerData.WizardList[wizIndex] = new WizardData
        {
            WizType = playerData.WizardList[wizIndex].WizType,
            RoomPositionX = playerData.WizardList[wizIndex].RoomPositionX,
            RoomPositionY = playerData.WizardList[wizIndex].RoomPositionY,
            Happiness = playerData.WizardList[wizIndex].Happiness + happinessAdjustmentAmount
        };
    }

    public VehicleData CopyTankRoomConstellationToVehicleData(TankRoomConstellation matrixToCopy)
    {
        VehicleData data = new VehicleData();
        data._savedXSize = matrixToCopy._savedXSize;
        data._savedYSize = matrixToCopy._savedYSize;

        data.RoofColorR = matrixToCopy.RoofColorR;
        data.RoofColorG = matrixToCopy.RoofColorG;
        data.RoofColorB = matrixToCopy.RoofColorB;

        data.FloorColorR = matrixToCopy.FloorColorR;
        data.FloorColorG = matrixToCopy.FloorColorG;
        data.FloorColorB = matrixToCopy.FloorColorB;

        data.VehicleMatrix = new PlayerVehicleMatrix(matrixToCopy._savedXSize, matrixToCopy._savedYSize);

        for (int x = 0; x < data._savedXSize; x++)
        {
            for (int y = 0; y < data._savedYSize; y++)
            {
                data.VehicleMatrix.XArray[x].YStuff[y].RoomPrefabPath = matrixToCopy._savedMatrix.XArray[x].YStuff[y].RoomPrefabPath;
                data.VehicleMatrix.XArray[x].YStuff[y].FloorType = matrixToCopy._savedMatrix.XArray[x].YStuff[y].FloorType;
                data.VehicleMatrix.XArray[x].YStuff[y].RoofType = matrixToCopy._savedMatrix.XArray[x].YStuff[y].RoofType;
                data.VehicleMatrix.XArray[x].YStuff[y].SystemPrefabPath = matrixToCopy._savedMatrix.XArray[x].YStuff[y].SystemPrefabPath;
                data.VehicleMatrix.XArray[x].YStuff[y].MovementPrefabPath = matrixToCopy._savedMatrix.XArray[x].YStuff[y].MovementPrefabPath;
                data.VehicleMatrix.XArray[x].YStuff[y].SystemDirection = matrixToCopy._savedMatrix.XArray[x].YStuff[y].SystemDirection;

                data.VehicleMatrix.XArray[x].YStuff[y]._topWallExists = matrixToCopy._savedMatrix.XArray[x].YStuff[y]._topWallExists;
                data.VehicleMatrix.XArray[x].YStuff[y]._rightWallExists = matrixToCopy._savedMatrix.XArray[x].YStuff[y]._rightWallExists;
                data.VehicleMatrix.XArray[x].YStuff[y]._bottomWallExists = matrixToCopy._savedMatrix.XArray[x].YStuff[y]._bottomWallExists;
                data.VehicleMatrix.XArray[x].YStuff[y]._leftWallExists = matrixToCopy._savedMatrix.XArray[x].YStuff[y]._leftWallExists;
            }
        }

        return data;
    }

    public TankRoomConstellation CopyVehicleDataToTankRoomConstellation(VehicleData data)
    {
        TankRoomConstellation tankRoom = new TankRoomConstellation();
        tankRoom._savedXSize = data._savedXSize;
        tankRoom._savedYSize = data._savedYSize;

        tankRoom.RoofColorR = data.RoofColorR;
        tankRoom.RoofColorG = data.RoofColorG;
        tankRoom.RoofColorB = data.RoofColorB;

        tankRoom.FloorColorR = data.FloorColorR;
        tankRoom.FloorColorG = data.FloorColorG;
        tankRoom.FloorColorB = data.FloorColorB;

        tankRoom._tmpMatrix = new TankRoomConstellation.XValues(data._savedXSize, data._savedYSize);

        for (int x = 0; x < tankRoom._savedXSize; x++)
        {
            for (int y = 0; y < tankRoom._savedYSize; y++)
            {
                tankRoom._tmpMatrix.XArray[x].YStuff[y].RoomPrefabPath = data.VehicleMatrix.XArray[x].YStuff[y].RoomPrefabPath;
                tankRoom._tmpMatrix.XArray[x].YStuff[y].FloorType = data.VehicleMatrix.XArray[x].YStuff[y].FloorType;
                tankRoom._tmpMatrix.XArray[x].YStuff[y].RoofType = data.VehicleMatrix.XArray[x].YStuff[y].RoofType;
                tankRoom._tmpMatrix.XArray[x].YStuff[y].SystemPrefabPath = data.VehicleMatrix.XArray[x].YStuff[y].SystemPrefabPath;
                tankRoom._tmpMatrix.XArray[x].YStuff[y].MovementPrefabPath = data.VehicleMatrix.XArray[x].YStuff[y].MovementPrefabPath;
                tankRoom._tmpMatrix.XArray[x].YStuff[y].SystemDirection = data.VehicleMatrix.XArray[x].YStuff[y].SystemDirection;

                tankRoom._tmpMatrix.XArray[x].YStuff[y]._topWallExists = data.VehicleMatrix.XArray[x].YStuff[y]._topWallExists;
                tankRoom._tmpMatrix.XArray[x].YStuff[y]._rightWallExists = data.VehicleMatrix.XArray[x].YStuff[y]._rightWallExists;
                tankRoom._tmpMatrix.XArray[x].YStuff[y]._bottomWallExists = data.VehicleMatrix.XArray[x].YStuff[y]._bottomWallExists;
                tankRoom._tmpMatrix.XArray[x].YStuff[y]._leftWallExists = data.VehicleMatrix.XArray[x].YStuff[y]._leftWallExists;
            }
        }
        return tankRoom;
    }
}
