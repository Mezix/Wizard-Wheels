using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class MainMenuCamera : MonoBehaviour
{
    //  Camera
    [HideInInspector]
    public Camera cam;
    private Transform objectToTrack;
    [SerializeField]
    private PixelPerfectCamera pixelCam;
    private int zoomLevel;
    [HideInInspector] public int closestZoom;
    [HideInInspector] public int furthestZoom;
    private int zoomAmount;

    private void Awake()
    {
        REF.mCam = this;
        cam = GetComponent<Camera>();
        //pixelCam = GetComponent<PixelPerfectCamera>();
    }
    private void Start()
    {
        zoomAmount = 5;
        closestZoom = 300;
        furthestZoom = 30; //lowest is 1
        pixelCam.assetsPPU = zoomLevel = furthestZoom;
        objectToTrack = REF.mMenu.orb.transform;
        SetCamParent(transform);
        cam.transform.localPosition = new Vector3(0, 0, -10);
    }
    private void FixedUpdate()
    {
        ZoomSlowly();
        MoveCamToObjectSlowly();
    }
    public void SetCamParent(Transform t)
    {
        objectToTrack = t;
    }
    public void SetZoom(int zoom)
    {
        zoomLevel = zoom;
    }
    private void ZoomSlowly()
    {
        pixelCam.assetsPPU += Math.Sign(zoomLevel - pixelCam.assetsPPU) * zoomAmount;
    }
    private void MoveCamToObjectSlowly()
    {
        //  Perfectly Track when we get close enough
        if (Vector3.Distance(cam.transform.position, objectToTrack.transform.position) < 0.05f)
        {
            Vector3 newPosition = objectToTrack.transform.position;
            newPosition.z = -10;
            cam.transform.position = newPosition;
        }
        //  Otherwise slowly get closer
        else
        {
            Vector3 newPosition = Vector2.Lerp(cam.transform.position, objectToTrack.transform.position, 0.1f);
            newPosition.z = -10;
            cam.transform.position = newPosition;
        }
    }
}
