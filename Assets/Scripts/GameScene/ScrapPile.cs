using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPile : MonoBehaviour
{
    public int scrapValue = 0;
    public void InitScrap(int val)
    {
        scrapValue = val;
    }
    public void PickUpScrap()
    {
        Ref.UI._upgradeScreen.AddNewScrap(scrapValue, true);
        RemoveScrap();
    }
    private void RemoveScrap()
    {
        Destroy(gameObject);
    }
}
