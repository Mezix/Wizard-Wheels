using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraScript : MonoBehaviour
{
    public Transform objToTrack;
    private Vector2 cameraOffset;
    private Vector3 mouseStartDragPos;
    private bool isMovingToPos;

    private PixelPerfectCamera pixelCam;
    public int zoomAmount;
    public int desiredZoom;
    public int maxZoom;
    public int minZoom;

    private void Awake()
    {
        Ref.Cam = this;
        pixelCam = Camera.main.GetComponent<PixelPerfectCamera>();
        isMovingToPos = false;
    }
    private void Start()
    {
        zoomAmount = 10;
        minZoom = 30;
        desiredZoom = maxZoom = 350;
        SetZoom(maxZoom);
        Events.instance.PlayerIsDying += StopTracking;
        Events.instance.PlayerTankDestroyed += StopTracking;
        Events.instance.EnemyTankDestroyed += CheckForEnemy;
        SetTrackedVehicleToPlayer();
    }
    
    private void CheckForEnemy(GameObject enemy)
    {
        if (enemy.transform.Equals(objToTrack)) SetTrackedVehicleToPlayer();
    }

    void Update()
    {
        HandleZoomInput();
        ZoomToDesiredZoom();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown(KeyCode.Mouse2)) mouseStartDragPos = Input.mousePosition; //initially set from where we are draggin
            if(Input.GetKey(KeyCode.Mouse2)) ChangeCameraOffset(); //dragging motion
        }
        if (objToTrack) MoveCameraToLocalPos();
    }

    

    //  Cam Movement

    public void SetTrackedVehicleToPlayer()
    {
        if (!Ref.PCon) return;
        objToTrack = Ref.PlayerGO.transform;
        transform.SetParent(objToTrack);
        cameraOffset = Vector3.zero;
        isMovingToPos = true;
    }
    public void SetTrackedVehicleToObject(Transform t)
    {
        objToTrack = t;
        transform.SetParent(objToTrack);
        HM.RotateTransformToAngle(transform, Vector3.zero);
        cameraOffset = Vector3.zero;
        isMovingToPos = true;
    }
    private void MoveCameraToLocalPos()
    {
        HM.RotateTransformToAngle(transform, Vector3.zero);
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
        objToTrack = null;
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

    //  Zoom

    private void HandleZoomInput()
    {
        if (Input.mouseScrollDelta.y != 0 && !Ref.UI._settingsOn)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                ZoomIn();
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                ZoomOut();
            }
        }
    }
    private void ZoomToDesiredZoom()
    {
        if (desiredZoom + zoomAmount < maxZoom)
        {
            SetZoom(desiredZoom + zoomAmount);
        }
        else SetZoom(maxZoom);
        if (desiredZoom - zoomAmount > minZoom)
        {
            SetZoom(desiredZoom - zoomAmount);
        }
        else SetZoom(minZoom);
    }
    private void ZoomIn()
    {
        if (desiredZoom + zoomAmount > maxZoom) desiredZoom = maxZoom;
        else desiredZoom += zoomAmount; 
    }
    private void ZoomOut()
    {
        if (desiredZoom - zoomAmount < minZoom) desiredZoom = minZoom;
        else desiredZoom -= zoomAmount;
    }
    private void SetZoom(int zoomLevel)
    {
        pixelCam.assetsPPU = zoomLevel;
    }
    public void SetDesiredZoom(int zoom)
    {
        if (desiredZoom > maxZoom) desiredZoom = maxZoom;
        else if (desiredZoom < minZoom) desiredZoom = minZoom;
        else desiredZoom = zoom;
    }
}
