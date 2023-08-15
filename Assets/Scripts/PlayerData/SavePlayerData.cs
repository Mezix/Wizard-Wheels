using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using static PlayerData;

public static class SavePlayerData
{
    public readonly static string path = Application.persistentDataPath;

    public static void SavePlayer (int saveSlot, PlayerData data)
    {
        string saveSlotPath = path + "/player" + saveSlot + ".save";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveSlotPath, FileMode.Create);
        formatter.Serialize(stream, data);

        stream.Close();

        Debug.Log("SAVE COMPLETE!");
    }

    public static PlayerData LoadPlayer (int saveSlot)
    {
        string saveSlotPath = path + "/player" + saveSlot + ".save";

        if (File.Exists(saveSlotPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(saveSlotPath, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            Debug.Log("LOAD COMPLETE!");
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + path + "/player" + saveSlot + ".save");
            return null;
        }
    }

    public static void DeleteSaveFile(int saveSlot)
    {
        string saveSlotPath = path + "/player" + saveSlot + ".save";

        if (File.Exists(saveSlotPath))
        {
            File.Delete(saveSlotPath);
        }
        else
        {
            Debug.LogWarning("Trying to delete save file at '" + path + "/player" + saveSlot + ".save'  failed! Does not exist!");
        }
    }

    public static PlayerData GenerateFreshSaveFile(int saveSlot)
    {
        List<InventoryItemData> freshInventory = new List<InventoryItemData>();
        List<EventNode> freshRoute = GenerateRandomRoute(5);
        float timePlayed = 0;


        UnityEngine.Object[] inventoryItemTypeList = Resources.LoadAll(GS.ScriptableObjects("InventoryItems"), typeof(InventoryItem));

        foreach (InventoryItem item in inventoryItemTypeList)
        {
            InventoryItemData tmpItem = new InventoryItemData();
            tmpItem.Name = item.Name;
            tmpItem.SpritePath = GS.InventoryGraphics(item.Image.name);
            tmpItem.Amount = 0;
            freshInventory.Add(tmpItem);
        }

        PlayerData freshPlayerData = new PlayerData(freshInventory, freshRoute, timePlayed, 0);
        SavePlayer(saveSlot, freshPlayerData);
        return freshPlayerData;
    }
}
