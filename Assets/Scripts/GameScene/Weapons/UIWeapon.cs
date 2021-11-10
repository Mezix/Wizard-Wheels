using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour
{
    public AWeapon _weapon;
    public Image _weaponImage;
    public int _index;
    public Text _UIWeaponName;
    public Text _UIWeaponIndex;
    public Image _UIWeaponCharge;
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    public void SelectWeapon()
    {
        if (_weapon != null) _weapon.WeaponSelected = true;
    }
    public void WeaponInteractable(bool b)
    {
        button.interactable = b;
    }
}
