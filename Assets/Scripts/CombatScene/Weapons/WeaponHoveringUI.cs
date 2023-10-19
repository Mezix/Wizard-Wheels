using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHoveringUI : MonoBehaviour
{
    public GameObject _allObjectsParent;
    public Transform _weaponRotationParent;
    public Image _weaponRotationLeftImage;
    public Transform _weaponRotationToGoTo;

    public void ShowUI(bool show)
    {
        if(_allObjectsParent) _allObjectsParent.SetActive(show);
    }
    public void SetRotation(float currentRotation, float rotationToGetTo)
    {
        float difference;
        if (currentRotation > rotationToGetTo) difference = currentRotation - rotationToGetTo;
        else difference = rotationToGetTo - currentRotation;

        bool rotateRight = currentRotation > rotationToGetTo;
        if (rotateRight)
        {
            HM.RotateLocalTransformToAngle(_weaponRotationLeftImage.transform, new Vector3(0, 180, 180));
        }
        else
        {
            HM.RotateLocalTransformToAngle(_weaponRotationLeftImage.transform, new Vector3(0, 0, 180));
        }
        HM.RotateLocalTransformToAngle(_weaponRotationParent, new Vector3(0, 0, rotationToGetTo));
        HM.RotateLocalTransformToAngle(_weaponRotationToGoTo, new Vector3(0, 0, currentRotation));
        _weaponRotationLeftImage.fillAmount = Mathf.Abs(difference / 360f);
    }
}