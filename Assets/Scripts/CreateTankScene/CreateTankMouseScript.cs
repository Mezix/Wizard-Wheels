using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateTankMouseScript : MonoBehaviour
{
    public Image _mouse;

    public string _mouseState;
    private void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        Cursor.visible = false;
        TrackMouse();
    }
    private void TrackMouse()
    {
        transform.position = Input.mousePosition;

        if(MouseCursor.IsPointerOverUIElement())
        {
            CreateTankSceneManager.instance.mouse._mouse.sprite = Resources.Load("Art\\UI\\Cursor", typeof(Sprite)) as Sprite;
        }
        else
        {
            if(_mouseState == "Brush") CreateTankSceneManager.instance.mouse._mouse.sprite = Resources.Load("Art\\UI\\Brush", typeof(Sprite)) as Sprite;
            if(_mouseState == "Eraser") CreateTankSceneManager.instance.mouse._mouse.sprite = Resources.Load("Art\\UI\\Eraser", typeof(Sprite)) as Sprite;
        }
    }
}