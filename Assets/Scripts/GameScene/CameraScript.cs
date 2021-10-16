using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform tankToTrack;
    private Vector2 cameraOffset;
    private Vector3 mouseStartDragPos;

    private void Awake()
    {
        Ref.Cam = this;
    }
    private void Start()
    {
        Events.instance.PlayerTankDestroyed += StopTracking;
        Events.instance.EnemyTankDestroyed += CheckForEnemy;
        SetTrackedVehicleToPlayer();
    }

    private void CheckForEnemy(GameObject enemy)
    {
        if (enemy.transform.Equals(tankToTrack)) SetTrackedVehicleToPlayer();
    }

    void Update()
    {
        if(tankToTrack) TrackCurrentTank();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown(KeyCode.Mouse2)) mouseStartDragPos = Input.mousePosition; //initially set from where we are draggin
            if(Input.GetKey(KeyCode.Mouse2)) ChangeCameraOffset(); //dragging motion
        }
    }
    public void SetTrackedVehicleToPlayer()
    {
        tankToTrack = Ref.PlayerGO.transform;
        cameraOffset = Vector3.zero;
    }
    public void SetTrackedVehicleToEnemy(Transform tankTransform)
    {
        tankToTrack = tankTransform;
        cameraOffset = Vector3.zero;
    }
    private void TrackCurrentTank()
    {
        transform.position = tankToTrack.position + new Vector3(cameraOffset.x, cameraOffset.y, -10);
    }
    private void StopTracking()
    {
        tankToTrack = null;
    }
    private void ChangeCameraOffset()
    {
        cameraOffset = new Vector3(cameraOffset.x, cameraOffset.y, 0) + 
                                   Camera.main.ScreenToWorldPoint(mouseStartDragPos) - 
                                   Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseStartDragPos = Input.mousePosition;
    }
}
