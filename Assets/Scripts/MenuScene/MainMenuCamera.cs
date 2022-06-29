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
    private Transform camParent;
    [SerializeField]
    private PixelPerfectCamera pixelCam;
    private int zoomLevel;
    [HideInInspector] public int closestZoom;
    [HideInInspector] public int furthestZoom;
    private int zoomAmount;

    private void Awake()
    {
        Ref.mCam = this;
        cam = GetComponent<Camera>();
        //pixelCam = GetComponent<PixelPerfectCamera>();
    }
    private void Start()
    {
        zoomAmount = 5;
        closestZoom = 300;
        furthestZoom = 30; //lowest is 1
        pixelCam.assetsPPU = zoomLevel = furthestZoom;
        camParent = Ref.mMenu.orb.transform;
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
        camParent = t;
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
        cam.transform.parent = camParent;
        Vector3 diff = Vector2.Lerp(cam.transform.localPosition, Vector2.zero, 0.1f);
        if (diff.magnitude < 0.01f) diff = Vector2.zero;
        diff.z = -10;
        cam.transform.localPosition = diff;
    }
}
