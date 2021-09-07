using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public static UIScript instance;

    public Button CruiseButton;
    public Button RotateBackButton;
    public Slider AccelerationSlider;

    private void Awake()
    {
        instance = this;
    }
}
