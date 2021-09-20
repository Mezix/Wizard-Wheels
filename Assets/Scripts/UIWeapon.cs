using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour
{
    public IWeapon weapon;
    public Image weaponImage;
    public int index;
    public Text UIWeaponName;
    public Text UIWeaponIndex;

    public void SelectWeapon()
    {
        if (weapon != null) weapon.weaponSelected = true;
    }
}
