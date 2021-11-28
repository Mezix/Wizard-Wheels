using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform tankToTrack;
    private Vector2 cameraOffset;
    private Vector3 mouseStartDragPos;
    private bool isMovingToPos;

    private void Awake()
    {
        Ref.Cam = this;
        isMovingToPos = false;
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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown(KeyCode.Mouse2)) mouseStartDragPos = Input.mousePosition; //initially set from where we are draggin
            if(Input.GetKey(KeyCode.Mouse2)) ChangeCameraOffset(); //dragging motion
        }
        if (tankToTrack) MoveCameraToLocalPos();
    }
    public void SetTrackedVehicleToPlayer()
    {
        if (!Ref.PCon) return;
        tankToTrack = Ref.PlayerGO.transform;
        transform.SetParent(tankToTrack);
        cameraOffset = Vector3.zero;
        isMovingToPos = true;
    }
    public void SetTrackedVehicleToEnemy(Transform tankTransform)
    {
        tankToTrack = tankTransform;
        transform.SetParent(tankToTrack);
        cameraOffset = Vector3.zero;
        isMovingToPos = true;
    }
    private void MoveCameraToLocalPos()
    {
        if (isMovingToPos)
        {
            Vector3 newPos = Vector2.Lerp(transform.localPosition, Vector3.zero, 0.1f);
            newPos.z = -10;
            if (Vector3.Distance(Vector3.zero, newPos) < 1f)
            {
                newPos = Vector3.zero;
                isMovingToPos = false;
            }
            transform.localPosition = newPos;
        }
        else
        {
            transform.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, -10);
        }
    }
    private void StopTracking()
    {
        tankToTrack = null;
        isMovingToPos = false;
        transform.parent = null;
    }
    private void ChangeCameraOffset()
    {
        cameraOffset = new Vector3(cameraOffset.x, cameraOffset.y, 0) + 
                                   Camera.main.ScreenToWorldPoint(mouseStartDragPos) - 
                                   Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseStartDragPos = Input.mousePosition;
        isMovingToPos = false;
    }
}
