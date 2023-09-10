using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WizardUI : MonoBehaviour
{
    public GameObject _XRayOnObjects;
    public GameObject _XRayOffObjects;

    public Text _unitName;
    public Image _hpFillImage;
    public Image _xRaySprite;

    public void ShowXRay(bool xRayOn)
    {
        _XRayOnObjects.SetActive(xRayOn);
        _XRayOffObjects.SetActive(!xRayOn);
    }
}
