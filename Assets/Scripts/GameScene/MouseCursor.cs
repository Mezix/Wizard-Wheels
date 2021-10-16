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

    public RectTransform _cursorTransform;
    private Animator cursorAnimator;

    private void Awake()
    {
        Ref.mouse = this;
        mouseRend = GetComponentInChildren<SpriteRenderer>();
        pixelCam = Camera.main.GetComponent<PixelPerfectCamera>();
        cursorAnimator = _cursorTransform.GetComponent<Animator>();
    }
    void Start()
    {
        //Cursor.visible = false; //disable the unity default mouse cursor

        zoomAmount = 10;
        minZoom = 30;
        maxZoom = 350;
        selectionBox = new Rect();
        DrawVisual();
        SetZoom(maxZoom);
        cursorAnimator.SetBool("clicked", false);
    }

    void Update()
    {
        //TrackCursor();
        HandleMouseSelectionInput();
        HandleZoomIn();
        TrackMouse();
        HandleMouseAnimation();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectAllUnits();
        }
    }

    private void HandleZoomIn()
    {
        if(Input.mouseScrollDelta.y != 0 && !Ref.UI.settingsOn)
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
        }
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
        if (Ref.PDead) return;
        foreach(TechWizard wizard in PlayerTankController.instance._spawnedWizards)
        {
            if (selectionBox.Contains(Camera.main.WorldToScreenPoint(wizard.transform.position)))
            {
                wizard.UnitSelected = true;
            }
        }
    }
    public void DeselectAllUnits()
    {
        if (Ref.PDead) return;
        foreach (TechWizard wizard in PlayerTankController.instance._spawnedWizards)
        {
            wizard.UnitSelected = false;
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

    //  Mouse Cursor

    private void HandleMouseAnimation()
    {
        if (Input.GetMouseButton(0))
        {
            cursorAnimator.SetBool("clicked", true);
        }
        else
        {
            cursorAnimator.SetBool("clicked", false);
        }
    }
    private void TrackMouse()
    {
        _cursorTransform.position = Input.mousePosition;
    }
}
