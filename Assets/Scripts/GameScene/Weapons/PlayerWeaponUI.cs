using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponUI : MonoBehaviour
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
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => SelectWeapon());
    }
    public void SelectWeapon()
    {
        if (!Input.GetKey(KeyCode.LeftShift)) Ref.PCon.TWep.DeselectAllWeapons();
        Events.instance.WizOrWepClicked(_weapon.gameObject);
        if (_weapon != null) _weapon.WeaponSelected = true;
    }
    public void WeaponInteractable(bool b)
    {
        button.interactable = b;
    }
}
