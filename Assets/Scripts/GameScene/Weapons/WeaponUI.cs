using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public int WeaponIndex { get; set; }
    public Text _weaponIndexText;
    [SerializeField]
    private GameObject weaponChargeBar;
    [SerializeField]
    private Image fill;

    public void ShowWeaponUI(bool player)
    {
        weaponChargeBar.SetActive(!player);
        if (player)
        {
            _weaponIndexText.transform.parent.localPosition = Vector3.zero;
        }
        else
        {
            _weaponIndexText.transform.parent.gameObject.SetActive(false);
        }
    }
    public void SetCharge(float pct)
    {
        pct = Mathf.Min(1, pct);
        fill.fillAmount = pct;
    }
}
