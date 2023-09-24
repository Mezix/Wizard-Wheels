﻿using Modern2D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SunScript : MonoBehaviour
{
    public Light2D _globalLight;
    public Gradient _nightCycleGradient;
    private float sunRotationSpeed;
    public float timeOfDay;
    private float dayStart;
    private float nightStart;
    private float maxShadowIntensity;

    void Start()
    {
        sunRotationSpeed = 1 * Time.fixedDeltaTime; // 1 degree per second -> 6 mins for day night cycle
        //sunRotationSpeed = 12 * Time.fixedDeltaTime; // 12 degrees per second -> 0.5 mins = 30s for day night cycle
        maxShadowIntensity = 0.7f;
        dayStart = 0f;
        nightStart = 160f;
        timeOfDay = HM.GetRandomInt(360);
    }
    void FixedUpdate()
    {
        UpdateSunPosition();
    }

    private void UpdateSunPosition()
    {
        LightingSystem.system.directionalLightAngle.value = -timeOfDay + 180;

        timeOfDay += sunRotationSpeed;
        if (timeOfDay >= 360) timeOfDay = 0;
        _globalLight.color = _nightCycleGradient.Evaluate(timeOfDay / 360f);

        if (timeOfDay >= dayStart && timeOfDay <= dayStart + 120)
        {
            LightingSystem.system._shadowAlpha.value = maxShadowIntensity * Mathf.Abs(Mathf.Sin(Mathf.PI / 2 + Mathf.PI * ((timeOfDay - dayStart) / 120f)));
        }
        else if (timeOfDay >= nightStart && timeOfDay <= nightStart + 120)
        {
            LightingSystem.system._shadowAlpha.value = maxShadowIntensity * Mathf.Abs(Mathf.Sin(Mathf.PI / 2 + Mathf.PI * ((timeOfDay - nightStart) / 120f)));
        }
        else
        {
            LightingSystem.system._shadowAlpha.value = maxShadowIntensity;
        }
        LightingSystem.system._shadowLength.value = LightingSystem.system._shadowAlpha * 5;
        LightingSystem.system._shadowLength.onValueChanged.Invoke();
    }
}
