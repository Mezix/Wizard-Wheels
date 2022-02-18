using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayer : MonoBehaviour
{
    public Image BG;
    public Image VisibilityImage;
    public int index;
    public bool _layerShown;

    public void ToggleVisibility()
    {
        _layerShown = !_layerShown;
        if (_layerShown) Show();
        else Hide();
    }
    public void Show()
    {
        _layerShown = true;
        VisibilityImage.sprite = Resources.Load("Art\\UI\\XRayOn", typeof(Sprite)) as Sprite;
        CreateTankSceneManager.instance._tUI.ShowLayer(true, index);
    }
    public void Hide()
    {
        _layerShown = false;
        VisibilityImage.sprite = Resources.Load("Art\\UI\\XRayOff", typeof(Sprite)) as Sprite;
        CreateTankSceneManager.instance._tUI.ShowLayer(false, index);
    }
}
