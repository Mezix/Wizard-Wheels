using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static bool paused;
    private void Awake()
    {
        References.TM = this;
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
        if (References.UI.settingsOn) return;
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
        References.UI.PauseImage.SetActive(paused);
    }
    public void UnpauseGame()
    {
        paused = false;
        References.UI.PauseImage.SetActive(paused);
    }
}
