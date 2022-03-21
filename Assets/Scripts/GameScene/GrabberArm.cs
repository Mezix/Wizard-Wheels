using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberArm : MonoBehaviour
{
    public bool _armLaunched;
    public GameObject _clawObj;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            ToggleArm();
        }
        if(_armLaunched)
        {
            MoveClawToMouse();
        }
        else
        {
            _clawObj.transform.parent = transform;
            _clawObj.transform.localPosition = Vector3.zero;
        }

        RotateToMouse();
    }

    private void RotateToMouse()
    {
        float angle = HM.GetAngle2DBetween(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
        HM.RotateTransformToAngle(_clawObj.transform, new Vector3(0,0,angle));
    }

    private void MoveClawToMouse()
    {
        _clawObj.transform.parent = null;
        _clawObj.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _clawObj.transform.position = new Vector3(_clawObj.transform.position.x, _clawObj.transform.position.y, 0);
    }
    private void ToggleArm()
    {
        if(!_armLaunched)
        {
            LaunchGrabberArm();
        }
        else
        {
            RetractGrabberArm();
        }
    }
    public void LaunchGrabberArm()
    {
        _armLaunched = true;
        print("launch");
    }
    public void RetractGrabberArm()
    {
        _armLaunched = false;
        print("retract");
    }

    //TODO add the connected points package to the arm as an extension!
}
