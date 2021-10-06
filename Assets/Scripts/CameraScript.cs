using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform vehicle;

    private void Start()
    {
        Events.instance.PlayerTankDestroyed += StopTrackingPlayer;
    }
    private void StopTrackingPlayer()
    {
        vehicle = null;
    }

    void Update()
    {
        if(vehicle) TrackTank();
    }
    private void TrackTank()
    {
        transform.position = vehicle.position + new Vector3(0,0,-10);
    }
}
