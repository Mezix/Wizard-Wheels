using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static bool paused;
    void Start()
    {
        paused = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleTime();
        }
    }
    public void ToggleTime()
    {
        paused = !paused;
        if (paused) Time.timeScale = 0;
        else Time.timeScale = 1;

        UIScript.instance.PauseImage.SetActive(paused);
    }
}
