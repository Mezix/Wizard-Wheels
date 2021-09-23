using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MouseCursor : MonoBehaviour
{
    private SpriteRenderer mouseRend;

    [SerializeField]
    private RectTransform boxVisual;

    private Vector2 startPosition;
    private Vector2 endPosition;

    private Rect selectionBox;
    private PixelPerfectCamera pixelCam;
    public int zoomAmount;
    public int maxZoom;
    public int minZoom;

    private void Awake()
    {
        mouseRend = GetComponentInChildren<SpriteRenderer>();
        pixelCam = Camera.main.GetComponent<PixelPerfectCamera>();
    }
    void Start()
    {
        //Cursor.visible = false; //disable the unity default mouse cursor

        zoomAmount = 10;
        minZoom = 30;
        maxZoom = 200;
        selectionBox = new Rect();
        DrawVisual();
        SetZoom(maxZoom);
    }

    void Update()
    {
        //TrackCursor();
        HandleMouseSelectionInput();
        HandleZoomIn();
    }
    /// <summary>
    /// Tracks the defined cursor to the mouses screen position
    /// </summary>
    public void TrackCursor()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
    }

    private void HandleZoomIn()
    {
        if(Input.mouseScrollDelta.y != 0 && !UIScript.instance.settingsOn)
        {
            if(Input.mouseScrollDelta.y > 0)
            {
                ZoomIn();
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                ZoomOut();
            }
        }
    }

    public void HandleMouseSelectionInput()
    {
        //Check for selecting ui first!

        /*
        if(Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            DrawVisual();
            DrawSelection();

        }
        if (Input.GetMouseButtonUp(0))
        {
            SelectUnits();
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            DrawVisual();
        }*/
    }
    public void DrawVisual()
    {
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;

        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        boxVisual.position = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

        boxVisual.sizeDelta = boxSize;
    }
    public void DrawSelection()
    {
        if(Input.mousePosition.x < startPosition.x)
        {
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = Input.mousePosition.x;
        }
        if (Input.mousePosition.y < startPosition.y)
        {
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }

    private void SelectUnits()
    {
        foreach(Wizard wizard in PlayerTankController.instance.wizardList)
        {
            if (selectionBox.Contains(Camera.main.WorldToScreenPoint(wizard.transform.position)))
            {
                wizard.unitSelected = true;
            }
        }
    }

    //  Zooming

    private void ZoomIn()
    {
        if (pixelCam.assetsPPU + zoomAmount > maxZoom) SetZoom(maxZoom);
        else SetZoom(pixelCam.assetsPPU += zoomAmount);
    }
    private void ZoomOut()
    {
        if (pixelCam.assetsPPU - zoomAmount < minZoom) SetZoom(minZoom);
        else SetZoom(pixelCam.assetsPPU -= zoomAmount);
    }
    private void SetZoom(int zoomLevel)
    {
        pixelCam.assetsPPU = zoomLevel;
    }
}
