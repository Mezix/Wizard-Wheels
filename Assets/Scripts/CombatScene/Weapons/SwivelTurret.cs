using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SwivelTurret : AWeapon
{
    public override void Start()
    {
        base.Start();
        AngleToAimAt = 0;
    }
}
