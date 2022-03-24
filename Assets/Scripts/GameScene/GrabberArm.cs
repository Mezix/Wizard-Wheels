using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberArm : MonoBehaviour
{
    public bool _armLaunched;
    public Transform _grabberArmCrossbowBody;
    public Transform _chainStartingPos;
    public GrabberClaw _clawScript;
    [SerializeField]
    private Animator _grabberArmAnimator;
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
            _clawScript.transform.parent = _grabberArmCrossbowBody;
            _clawScript.transform.localPosition = Vector3.zero;
        }
        RotateToMouse();
        CreateChain();
    }

    private void CreateChain()
    {
        Sprite chain = Resources.Load("Art/Weapons/chain_link", typeof (Sprite)) as Sprite;
        DottedLine.DottedLine.Instance.DrawDottedLine(_chainStartingPos.position, _clawScript._clawAnimator.transform.position, Color.white, chain);
    }

    private void RotateToMouse()
    {
        float angle = HM.GetAngle2DBetween(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
        HM.RotateTransformToAngle(_grabberArmCrossbowBody.transform, new Vector3(0, 0, angle));
        HM.RotateTransformToAngle(_clawScript.transform, new Vector3(0, 0, angle));
    }

    private void MoveClawToMouse()
    {
        _clawScript.transform.parent = null;
        _clawScript.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _clawScript.transform.position = new Vector3(_clawScript.transform.position.x, _clawScript.transform.position.y, 0);
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
        _grabberArmAnimator.SetTrigger("FireCrossbow");
        _armLaunched = true;
    }
    public void RetractGrabberArm()
    {
        _armLaunched = false;
    }

    //TODO add the connected points package to the arm as an extension!
}
