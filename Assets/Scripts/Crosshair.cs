using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Text weaponIndex;
    public void SetCrosshairWeaponIndex(string txt)
    {
        weaponIndex.text = txt;
    }
}
