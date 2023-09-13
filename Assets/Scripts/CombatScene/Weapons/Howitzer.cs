using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Howitzer : AWeapon
{
    public override void Start()
    {
        base.Start();
        ProjectilePrefab = Resources.Load(GS.WeaponPrefabs("HowitzerBallPrefab"), typeof(GameObject)) as GameObject;
    }
}
