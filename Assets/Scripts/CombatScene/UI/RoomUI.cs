using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    public GameObject roomUIObjects;
    public Image _repairFill;
    public Image _repairIndicator;
    private void Awake()
    {
        roomUIObjects.gameObject.SetActive(false);
        UpdateRepair(0);
    }
    public void NeedsUnitToRepair(bool repairStatus)
    {
        _repairIndicator.gameObject.SetActive(repairStatus);
    }
    public void UpdateRepair(float pct)
    {
        _repairFill.fillAmount = pct;

        if (pct > 0) _repairFill.transform.parent.gameObject.SetActive(true);
        else _repairFill.transform.parent.gameObject.SetActive(false);
    }
}
