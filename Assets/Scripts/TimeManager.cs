using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public static bool paused;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        paused = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) TogglePauseWhilstPlaying();
    }
    public void TogglePauseWhilstPlaying()
    {
        if (UIScript.instance.settingsOn) return;
        if (!paused)
        {
            FreezeTime();
            PauseGame();
        }
        else
        {
            UnfreezeTime();
            UnpauseGame();
        }
    }

    public void FreezeTime()
    {
        Time.timeScale = 0;
    }
    public void UnfreezeTime()
    {
        Time.timeScale = 1;
    }
    public void PauseGame()
    {
        paused = true;
        UIScript.instance.PauseImage.SetActive(paused);
    }
    public void UnpauseGame()
    {
        paused = false;
        UIScript.instance.PauseImage.SetActive(paused);
    }
}
