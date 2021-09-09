using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public static UIScript instance;
    public static bool paused;

    public Button CruiseButton;
    public Button RotateBackButton;
    public Slider AccelerationSlider;

    public GameObject PauseImage;
    public GameObject SteeringWheel;

    private void Awake()
    {
        paused = false;
        PauseImage.SetActive(false);
        instance = this;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ToggleTime();
        }
    }
    public void ToggleTime()
    {
        paused = !paused;
        if (paused) Time.timeScale = 0;
        else Time.timeScale = 1;

        PauseImage.SetActive(paused);
    }
}
