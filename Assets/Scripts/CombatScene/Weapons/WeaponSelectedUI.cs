using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectedUI : MonoBehaviour
{
    public LineRenderer _selectedLR;
    public SpriteRenderer _outOfRangeIndicator;
    public Text _distanceText;
    private AWeapon weapon;
    public Image _weaponRangeCircle;

    public void InitWeaponSelectedUI(AWeapon wep)
    {
        weapon = wep;
        _weaponRangeCircle.transform.localScale = 2 * Vector3.one * wep.MaxLockOnRange;
        _weaponRangeCircle.fillAmount = wep._weaponStats._maxSwivel/360f;
        HM.RotateLocalTransformToAngle(_weaponRangeCircle.transform, new Vector3(0, 0, 180 - wep._maxAllowedAngleToTurn/2f));
    }
    public void UpdateWeaponSelectedLR()
    {
        _weaponRangeCircle.gameObject.SetActive(false);
        if (weapon.WeaponSelected && weapon.WeaponEnabled &&!weapon.ShouldHitPlayer)
        {
            _weaponRangeCircle.gameObject.SetActive(true);
            Vector3 weaponPos = weapon.RotatablePart.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float distance = Vector3.Distance(weaponPos, mousePos);

            if (_selectedLR)
            {
                _selectedLR.SetPosition(0, weaponPos);
                _selectedLR.SetPosition(1, mousePos);
                _selectedLR.transform.parent.gameObject.SetActive(true);
            }
            if (_outOfRangeIndicator)
            {
                if (distance <= weapon.MaxLockOnRange)
                {
                    _outOfRangeIndicator.enabled = false;
                }
                else
                {
                    _outOfRangeIndicator.enabled = true;
                    float tankRotationZ = HM.WrapAngle(weapon.tMov.transform.localRotation.eulerAngles.z);
                    float weaponRotation = HM.WrapAngle(weapon.transform.localRotation.eulerAngles.z);
                    _outOfRangeIndicator.transform.localPosition = HM.Get2DCartesianFromPolar(-tankRotationZ + 180 - weaponRotation + HM.GetEulerAngle2DBetween(weapon.RotatablePart.transform.position, mousePos), weapon.MaxLockOnRange);
                    HM.RotateTransformToAngle(_outOfRangeIndicator.transform, Vector3.zero);
                }
            }
            _distanceText.text = Mathf.RoundToInt(distance) + " M";
        }
        else
        {
            _selectedLR.transform.parent.gameObject.SetActive(false);
            _outOfRangeIndicator.transform.localPosition = Vector3.zero;
            _outOfRangeIndicator.enabled = false;
        }
        _outOfRangeIndicator.transform.localPosition = new Vector3(_outOfRangeIndicator.transform.localPosition.x, _outOfRangeIndicator.transform.localPosition.y, 0);
    }
}
