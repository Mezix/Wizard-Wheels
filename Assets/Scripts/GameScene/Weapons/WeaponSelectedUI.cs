using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectedUI : MonoBehaviour
{
    [HideInInspector]
    public GameObject _targetingCircle;
    public LineRenderer _selectedLR;
    public SpriteRenderer _outOfRangeIndicator;
    public Text _distanceText;
    private AWeapon weapon;

    public void InitWeaponSelectedUI(AWeapon wep)
    {
        weapon = wep;
        GameObject go = Instantiate((GameObject)Resources.Load("WeaponRangeCircle"));
        go.transform.localScale = Vector3.one * wep.MaxLockOnRange;
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        _targetingCircle = go;
        _targetingCircle.SetActive(false);
    }
    public void UpdateWeaponSelectedLR()
    {
        _targetingCircle.SetActive(false);
        if (weapon.WeaponSelected && !weapon.ShouldHitPlayer)
        {
            _targetingCircle.SetActive(true);
            Vector3 weaponPos = weapon.transform.position;
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
                    _outOfRangeIndicator.transform.localPosition = (mousePos - weaponPos).normalized * weapon.MaxLockOnRange;
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
