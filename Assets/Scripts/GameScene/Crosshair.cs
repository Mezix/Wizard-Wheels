using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Canvas weaponTextCanvas;
    public Text weaponIndex;
    public Text tankName;
    public GameObject crosshair;
    public void SetCrosshairWeaponText(string index, string TankName = "")
    {
        weaponIndex.text = index;
        if(TankName != "")
        {
            tankName.text = TankName;
            tankName.gameObject.SetActive(true);
        }
    }
    public void SetCrosshairSizeAndPosition(int sizex, int sizey)
    {
        crosshair.transform.localScale = new Vector2(sizex, sizey);
        if (sizex == 2)
        {
            transform.localPosition = new Vector2(0.25f, -0.25f);
            weaponTextCanvas.transform.localPosition = new Vector2(0.25f, 0.25f);
        }
    }
}
