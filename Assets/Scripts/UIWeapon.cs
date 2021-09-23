using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour
{
    public IWeapon _weapon;
    public Image _weaponImage;
    public int _index;
    public Text _UIWeaponName;
    public Text _UIWeaponIndex;
    public Image _UIWeaponCharge;
    public void SelectWeapon()
    {
        if (_weapon != null) _weapon.WeaponSelected = true;
    }
}
