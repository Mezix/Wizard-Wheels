using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleExpanderWidget : MonoBehaviour
{
    public Button _top;
    public Button _down;
    public Button _left;
    public Button _right;

    public void SetWidgetDirection(bool add)
    {
        Vector3 flipVector = new Vector3(0,0,180);
        if(add)
        {
            HM.RotateLocalTransformToAngle(_top.transform, new Vector3(0, 0, 0));
            HM.RotateLocalTransformToAngle(_down.transform, new Vector3(0, 0, -180));
            HM.RotateLocalTransformToAngle(_left.transform, new Vector3(0, 0, 90));
            HM.RotateLocalTransformToAngle(_right.transform, new Vector3(0, 0, -90));
        }
        else
        {
            HM.RotateLocalTransformToAngle(_top.transform, new Vector3(0, 0, 0) + flipVector);
            HM.RotateLocalTransformToAngle(_down.transform, new Vector3(0, 0, -180) + flipVector);
            HM.RotateLocalTransformToAngle(_left.transform, new Vector3(0, 0, 90) + flipVector);
            HM.RotateLocalTransformToAngle(_right.transform, new Vector3(0, 0, -90) + flipVector);
        }
    }
}
