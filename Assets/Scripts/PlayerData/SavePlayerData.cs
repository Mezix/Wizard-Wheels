using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using static PlayerData;

public static class SavePlayerData
{
    public readonly static string path = Application.persistentDataPath;

    public static void SavePlayer ( int saveSlot, List<InventoryItemData> inventoryItems, List<EventNode> eventNodes, float timeInSecondsPlayed)
    {
        string saveSlotPath = path + "/player" + saveSlot + ".save";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveSlotPath, FileMode.Create);

        PlayerData data = new PlayerData(inventoryItems, eventNodes, timeInSecondsPlayed);

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
            Debug.LogError("Save file not found in " + path + "/player" + saveSlot + ".save");
            return null;
        }
    }
}
