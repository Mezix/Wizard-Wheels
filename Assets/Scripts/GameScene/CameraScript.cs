using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform vehicleToTrack;
    private Vector2 cameraOffset;
    private Vector3 mouseStartDragPos;
    private bool movingCameraOffset;

    private void Awake()
    {
    }
    private void Start()
    {
        Events.instance.PlayerTankDestroyed += StopTracking;
        SetTrackedVehicleToPlayer();
    }
    
    void Update()
    {
        if(vehicleToTrack) TrackCurrentTank();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown(KeyCode.Mouse2)) mouseStartDragPos = Input.mousePosition; //initially set from where we are draggin
            if(Input.GetKey(KeyCode.Mouse2)) ChangeCameraOffset(); //dragging motion
        }
        else movingCameraOffset = false;
    }
    public void SetTrackedVehicleToPlayer()
    {
        vehicleToTrack = References.PlayerGameObject.transform;
        cameraOffset = Vector3.zero;
    }
    public void SetTrackedVehicleToEnemy(Transform tankTransform)
    {
        vehicleToTrack = tankTransform;
        cameraOffset = Vector3.zero;
    }
    private void TrackCurrentTank()
    {
        transform.position = vehicleToTrack.position + new Vector3(cameraOffset.x, cameraOffset.y, -10);
    }
    private void StopTracking()
    {
        vehicleToTrack = null;
    }
    private void ChangeCameraOffset()
    {
        movingCameraOffset = true;

        cameraOffset = new Vector3(cameraOffset.x, cameraOffset.y, 0) + 
                                   Camera.main.ScreenToWorldPoint(mouseStartDragPos) - 
                                   Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseStartDragPos = Input.mousePosition;
        print(cameraOffset);
    }
}
