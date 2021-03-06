using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateTankMouseScript : MonoBehaviour
{
    public Image _mouse;
    public string _mouseState;

    private Vector2 cameraOffset;
    private Vector3 mouseStartDragPos;
    public float zoomAmount;
    public float desiredZoom;
    public float minZoom;
    public float maxZoom;
    private void Start()
    {
        Cursor.visible = false;
        zoomAmount = 0.75f;
        maxZoom = 1;
        minZoom = 12;
        desiredZoom = maxZoom;

        SetZoom(maxZoom);
    }
    void Update()
    {
        Cursor.visible = false;
        TrackMouse();

        HandleZoomInput();
        ZoomToDesiredZoom();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Mouse2)) mouseStartDragPos = Input.mousePosition; //initially set from where we are draggin
            if (Input.GetKey(KeyCode.Mouse2)) ChangeCameraOffset(); //dragging motion
        }
        Camera.main.transform.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, -10);
    }
    private void TrackMouse()
    {
        transform.position = Input.mousePosition;

        if(MouseCursor.IsPointerOverUIElement())
        {
            CreateTankSceneManager.instance.mouse._mouse.sprite = Resources.Load("Art\\UI\\Cursor", typeof(Sprite)) as Sprite;
            CreateTankSceneManager.instance._tools.previewTile = false;
        }
        else
        {
            if(_mouseState == "Brush")
            {
                CreateTankSceneManager.instance.mouse._mouse.sprite = Resources.Load("Art\\UI\\Brush", typeof(Sprite)) as Sprite;
                CreateTankSceneManager.instance._tools.previewTile = true;
            }
            if(_mouseState == "Eraser")
            {
                CreateTankSceneManager.instance.mouse._mouse.sprite = Resources.Load("Art\\UI\\Eraser", typeof(Sprite)) as Sprite;
                CreateTankSceneManager.instance._tools.previewTile = false;
            }
        }
    }
    private void ChangeCameraOffset()
    {
        cameraOffset = new Vector3(cameraOffset.x, cameraOffset.y, 0) +
                                   Camera.main.ScreenToWorldPoint(mouseStartDragPos) -
                                   Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseStartDragPos = Input.mousePosition;
    }

    //  Zooming

    private void ZoomToDesiredZoom()
    {
        if (desiredZoom + zoomAmount <= minZoom)
        {
            SetZoom(desiredZoom + zoomAmount);
        }
        else SetZoom(minZoom);
        if (desiredZoom - zoomAmount >= maxZoom)
        {
            SetZoom(desiredZoom - zoomAmount);
        }
        else SetZoom(maxZoom);
    }
    private void HandleZoomInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                ZoomOut();
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                ZoomIn();
            }
        }
    }
    private void ZoomIn()
    {
        if (desiredZoom + zoomAmount >= minZoom) desiredZoom = minZoom;
        else desiredZoom += zoomAmount;
    }
    private void ZoomOut()
    {
        if (desiredZoom - zoomAmount <= maxZoom) desiredZoom = maxZoom;
        else desiredZoom -= zoomAmount;
    }
    private void SetZoom(float zoomLevel)
    {
        Camera.main.orthographicSize = zoomLevel;
        //pixelCam.assetsPPU = zoomLevel;
    }
}