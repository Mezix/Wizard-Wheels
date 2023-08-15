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

        if (playerData.CurrentEventPathIndex >= playerData.CurrentEventPath.Count) Loader.Load(Loader.Scene.VictoryScene);
        else Loader.Load(Loader.Scene.RouteTransitionScene);
    }
}
