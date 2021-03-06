using System;
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
    public List<AWeapon> AttackingWeapons;
    public List<GameObject> WeaponIndices;

    private void Awake()
    {
        AttackingWeapons = new List<AWeapon>();
        WeaponIndices = new List<GameObject>();
    }

    public void AddAttacker(AWeapon weapon)
    {
        if(!AttackingWeapons.Contains(weapon))
        {
            AttackingWeapons.Add(weapon);
            UpdateCrosshair();
        }
    }
    public bool RemoveAttacker(AWeapon wep)
    {
        if (!AttackingWeapons.Contains(wep)) return false;
        AttackingWeapons.Remove(wep);
        UpdateCrosshair();
        if (AttackingWeapons.Count == 0)
        {
            //if our list of attackers is empty, tell our Crosshairmanager to despawn the crosshair by returning true
            return true;
        }
        return false;
    }
    private void DeleteAllIndices()
    {
        foreach(GameObject g in WeaponIndices)
        {
            Destroy(g);
        }
        WeaponIndices.Clear();
    }
    private void UpdateCrosshair()
    {
        DeleteAllIndices();

        float radius = 50 * crosshair.transform.localScale.x; //radius of the circle function used to calculate the crosshair index's position
        int i = 0; //a counter to shift the position of our crosshair indices
        float shiftAngle = 30;

        //Perhaps a sorting step by indices here?

        foreach(AWeapon weapon in AttackingWeapons)
        {
            //spawn a new Gameobject 
            GameObject wepIndexObject = Instantiate((GameObject)Resources.Load("WeaponIndex"));
            WeaponIndices.Add(wepIndexObject);
            wepIndexObject.transform.SetParent(weaponTextCanvas.transform, false);
            wepIndexObject.GetComponent<Text>().text = weapon.EnemyWepUI.WeaponIndex.ToString();

            //move the object on a circle
            wepIndexObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(radius * Mathf.Cos(Mathf.Deg2Rad * (45 - shiftAngle * i)),
                                                                                        radius * Mathf.Sin(Mathf.Deg2Rad * (45 - shiftAngle * i)));
            i++;
        }
    }
    public void SetCrosshairWeaponText(string index, string TankName = "")
    {
        weaponIndex.text = index;
        if (TankName != "")
        {
            tankName.text = TankName;
            tankName.gameObject.SetActive(true);
        }
    }
    public void SetCrosshairSizeAndPosition(int sizex, int sizey)
    {
        int minSize = Mathf.Min(sizex, sizey);
        crosshair.transform.localScale = new Vector2(minSize, minSize);
        transform.localPosition = Vector2.zero;

        transform.localPosition = new Vector2(0.25f * (sizex-1), -0.25f * (sizey-1));
        weaponTextCanvas.transform.localPosition = new Vector2(0.25f * (sizex - 1), 0.25f * (sizey - 1));
    }
}
