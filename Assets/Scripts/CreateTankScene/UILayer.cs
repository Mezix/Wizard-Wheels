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
        VisibilityImage.sprite = Resources.Load(GS.UIGraphics("XRayOn"), typeof(Sprite)) as Sprite;
        CreateTankUI.instance.ShowLayer(true, index);
    }
    public void Hide()
    {
        _layerShown = false;
        VisibilityImage.sprite = Resources.Load(GS.UIGraphics("XRayOff"), typeof(Sprite)) as Sprite;
        CreateTankUI.instance.ShowLayer(false, index);
    }
}
