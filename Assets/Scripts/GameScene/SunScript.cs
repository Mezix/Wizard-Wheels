using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D _sunLightSource;
    public Gradient _nightCycleGradient;
    private float sunRotationSpeed;
    public float timeOfDay;
    private float dayStart;
    private float nightStart;
    private float maxShadowIntensity;

    void Start()
    {
        //sunRotationSpeed = 1 * Time.fixedDeltaTime; // 1 degree per second -> 6 mins for day night cycle
        sunRotationSpeed = 12 * Time.fixedDeltaTime; // 12 degrees per second -> 0.5 mins for day night cycle
        maxShadowIntensity = 0.7f;
        dayStart = 0f;
        nightStart = 160f;
        timeOfDay = 0;
    }
    void FixedUpdate()
    {
        UpdateSunPosition();
    }

    private void UpdateSunPosition()
    {
        HM.RotateLocalTransformToAngle(transform, new Vector3(0, 0, transform.localRotation.eulerAngles.z - sunRotationSpeed));

        timeOfDay += sunRotationSpeed;
        if (timeOfDay >= 360) timeOfDay = 0;
        _sunLightSource.color = _nightCycleGradient.Evaluate(timeOfDay / 360f);

        if (timeOfDay >= dayStart && timeOfDay <= dayStart + 120)
        {
            _sunLightSource.shadowIntensity = maxShadowIntensity * Mathf.Abs(Mathf.Sin(Mathf.PI / 2 + Mathf.PI * ((timeOfDay - dayStart) / 120f)));
        }
        else if (timeOfDay >= nightStart && timeOfDay <= nightStart + 120)
        {
            _sunLightSource.shadowIntensity = maxShadowIntensity * Mathf.Abs(Mathf.Sin(Mathf.PI / 2 + Mathf.PI * ((timeOfDay - nightStart) / 120f)));
        }
        else
        {
            _sunLightSource.shadowIntensity = maxShadowIntensity;
        }
    }
}
