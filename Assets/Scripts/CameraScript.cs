using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform vehicle;
    void Update()
    {
        TrackTank();
    }
    private void TrackTank()
    {
        transform.position = vehicle.position + new Vector3(0,0,-10);
    }
}
