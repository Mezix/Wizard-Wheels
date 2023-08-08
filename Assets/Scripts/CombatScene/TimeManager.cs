using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static bool paused;
    private bool canPause;
    private void Awake()
    {
        REF.TM = this;
        canPause = true;
    }
    void Start()
    {
        paused = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canPause) TogglePauseWhilstPlaying();
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

    public void TriggerGradualSlowdown(float timeScaleToSlowDownTo)
    {
        canPause = false;
        StartCoroutine(SlowDownTo(timeScaleToSlowDownTo));
    }

    private IEnumerator SlowDownTo(float timeScaleToSlowDownTo)
    {
        Time.timeScale = 1;
        while (Time.timeScale > timeScaleToSlowDownTo)
        {
            Time.timeScale -= 0.05f;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        Time.timeScale = timeScaleToSlowDownTo;
    }
}
