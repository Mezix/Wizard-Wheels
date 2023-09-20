using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTarget : MonoBehaviour
{
    public SpriteRenderer _targetSprite;
    public LineRenderer _toWeaponLineRender;
    public AWeapon _assignedWeapon;
    private void Update()
    {
        HM.RotateLocalTransformToAngle(_toWeaponLineRender.transform, new Vector3(0,0, 180 + HM.GetEulerAngle2DBetween(transform.position, _assignedWeapon.transform.position)));
        _toWeaponLineRender.SetPosition(1, new Vector2(Vector2.Distance(transform.position, _assignedWeapon.transform.position), 0));
    }
}