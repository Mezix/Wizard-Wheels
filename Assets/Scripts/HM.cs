using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HM
{
    public static void RotateTransformToAngle(Transform t, Vector3 vec)
    {
        Quaternion q = new Quaternion();
        q.eulerAngles = vec;
        t.rotation = q;
    }
    public static float Angle2D(Vector3 a, Vector3 b)
    {
        return Mathf.Rad2Deg * Mathf.Atan2(a.y - b.y, a.x - b.x);
    }
}
