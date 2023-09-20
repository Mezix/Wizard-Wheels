using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AWeapon;

public class WeaponUI : MonoBehaviour
{
    public int WeaponIndex { get; set; }
    public GameObject _weaponIndexObject;
    public Text _weaponIndexText;
    [SerializeField]
    private GameObject weaponChargeBar;
    [SerializeField]
    private Slider fillSlider;
    [SerializeField]
    private Image fillImage;

    public Gradient _weaponChargeGradient;
    public Color _chargeColor;

    public void ShowWeaponUI(bool player)
    {
        weaponChargeBar.SetActive(!player);
        if (player)
        {
            _weaponIndexText.transform.parent.localPosition = Vector3.zero;
        }
        else
        {
            _weaponIndexObject.gameObject.SetActive(false);
        }
    }
    public void SetCharge(float pct, FiringStatus chargeMode)
    {
        pct = Mathf.Min(1, pct);
        fillSlider.value = pct;

        if (chargeMode.Equals(FiringStatus.Reloading))
        {
            fillImage.color = _weaponChargeGradient.Evaluate(pct);
            fillSlider.value = pct;
        }
        else if (chargeMode.Equals(FiringStatus.Charging))
        {
            fillImage.color = _chargeColor;
            fillSlider.value = 1 - pct;
        }
    }
}
