using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SavePlayerData
{
    public readonly static string path = Application.persistentDataPath;

    public static void SavePlayer (int saveSlot, int scrapAmount = -1)
    {
        string saveSlotPath = path + "/player" + saveSlot + ".save";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveSlotPath, FileMode.Create);

        PlayerData data = new PlayerData(scrapAmount);

        formatter.Serialize(stream, data);
        stream.Close();
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

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path + "/player" + saveSlot + ".save");
            return null;
        }
    }
}
