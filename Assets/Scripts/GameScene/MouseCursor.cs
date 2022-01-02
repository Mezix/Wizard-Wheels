using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseCursor : MonoBehaviour
{
    private SpriteRenderer mouseRend;
    private bool select;
    [SerializeField]
    private RectTransform boxVisual;

    private Vector2 startPosition;
    private Vector2 endPosition;

    private Rect selectionBox;

    public RectTransform _cursorTransform;
    private Animator cursorAnimator;
    [SerializeField]
    private List<GameObject> movementIndicators;

    private void Awake()
    {
        Ref.mouse = this;
        mouseRend = GetComponentInChildren<SpriteRenderer>();
        cursorAnimator = _cursorTransform.GetComponent<Animator>();
        movementIndicators = new List<GameObject>();
    }
    void Start()
    {
        //Cursor.visible = false; //disable the unity default mouse cursor

        boxVisual.gameObject.SetActive(false);
        selectionBox = new Rect();
        cursorAnimator.SetBool("clicked", false);
        InitMovementIndicators();
    }
    void Update()
    {
        HandleMouseSelectionInput();
        TrackMouse();
        HandleMouseAnimation();
        AttemptOutlineWizards();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectAllUnits();
        }
        ShowMovementIndicators();
    }

    private void AttemptOutlineWizards()
    {
        if (Ref.PCon._dying || Ref.PCon._dead) return;
        foreach (AUnit u in Ref.PCon._spawnedWizards)
        {
            u.DeHighlight();
        }
        RaycastHit2D hit = HM.RaycastToMouseCursor(LayerMask.GetMask("Wizard"));
        if (!hit.collider) return;
        if(hit.collider.transform.parent.TryGetComponent(out AUnit unit))
        {
            if(unit.CurrentRoom.tGeo.GetComponent<PlayerTankController>()) unit.Highlight();
        }
    }

    private void InitMovementIndicators()
    {
        int amount = 10;
        for(int i = 0; i < amount; i++)
        {
            movementIndicators.Add(Instantiate((GameObject)Resources.Load("MouseCursorMovingToIndicator")));
            movementIndicators[i].SetActive(false);
            movementIndicators[i].transform.parent = transform;
        }
    }

    private void ShowMovementIndicators()
    {
        int wizCount = 0;
        foreach(AUnit unit in Ref.PCon._spawnedWizards)
        {
            if (unit.UnitSelected) wizCount++;
        }
        foreach(GameObject g in movementIndicators)
        {
            g.transform.parent = transform;
            g.transform.localPosition = Vector3.zero;
            g.SetActive(false);
        }
        if (wizCount == 0) return;
        RaycastHit2D hit = HM.RaycastToMouseCursor(LayerMask.GetMask("Room"));

        if (!hit.collider) return;
        Room room = hit.collider.transform.GetComponent<Room>();
        if (room.tGeo.GetComponent<EnemyTankController>()) return;
        if (room)
        {
            List<RoomPosition> freeRoomPos = room.GetAllFreeRoomPos();
            if (freeRoomPos.Count == 0) return;

            int amountToSpawn = Mathf.Min(wizCount, freeRoomPos.Count);
            for(int i = 0; i < amountToSpawn; i++)
            {
                movementIndicators[i].transform.parent = freeRoomPos[i].transform;
                movementIndicators[i].transform.localPosition = Vector3.zero;
                movementIndicators[i].SetActive(true);
            }
        }
    }
    

    public void HandleMouseSelectionInput()
    {
        //Check for selecting ui first!
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            if (IsPointerOverUIElement())
            {
                boxVisual.gameObject.SetActive(false);
                select = false;
            }
            else
            {
                boxVisual.gameObject.SetActive(true);
                select = true;
            }
        }
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            DrawVisual();
            CreateRectSelection();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(select) SelectUnits();
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
    public void CreateRectSelection()
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
        foreach(AUnit wizard in Ref.PCon._spawnedWizards)
        {
            if (selectionBox.Contains(Camera.main.WorldToScreenPoint(wizard.UnitObj.transform.position)))
            {
                wizard.UnitSelected = true;
            }
        }
    }
    public void DeselectAllUnits()
    {
        if (Ref.PDead) return;
        foreach (AUnit wizard in Ref.PCon._spawnedWizards)
        {
            wizard.UnitSelected = false;
        }
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

    //Check if Mouse over UI
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
