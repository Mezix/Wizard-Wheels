using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int scrapAmount;
    public PlayerData(int scrap = -1)
    {
        if(scrap >= 0) scrapAmount = scrap; //only overwrite scrap if we are positive
    }
}
