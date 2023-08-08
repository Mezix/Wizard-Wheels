using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void FinishEvent()
    {
        if (playerData.CurrentEventPathIndex >= playerData.CurrentEventPath.Count)
        {
            Debug.LogWarning("Last event reached! No More events to load");
            return;
        }
        playerData.CurrentEventPath[playerData.CurrentEventPathIndex] = new PlayerData.EventNode(playerData.CurrentEventPath[playerData.CurrentEventPathIndex]._event, true);
        playerData.CurrentEventPathIndex++;

        SavePlayerData.SavePlayer(saveSlot, playerData);
        Loader.Load(Loader.Scene.RouteTransitionScene);
    }
    private void Update()
    {
        if(playerData != null) playerData.TimeInSecondsPlayed += Time.deltaTime;
    }
}
