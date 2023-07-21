using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static bool paused;
    private void Awake()
    {
        REF.TM = this;
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
        if (REF.UI._settingsScript._settingsOn) return;
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
        REF.UI._pauseImage.SetActive(paused);
    }
    public void UnpauseGame()
    {
        paused = false;
        REF.UI._pauseImage.SetActive(paused);
    }
}
