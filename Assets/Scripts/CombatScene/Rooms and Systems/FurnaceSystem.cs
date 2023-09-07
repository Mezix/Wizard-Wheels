using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceSystem : ASystem
{
    public Transform _smoke;
    public override void SpawnInCorrectDirection()
    {
        base.SpawnInCorrectDirection();
        HM.RotateTransformToAngle(_smoke, Vector3.zero);
    }
}
