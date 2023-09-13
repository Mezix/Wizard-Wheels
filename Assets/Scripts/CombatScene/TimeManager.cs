using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static bool paused;
    private bool canPause;
    private IEnumerator currentHitStop = null;
    public float timeScale;
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
        timeScale = Time.timeScale;
        if (Input.GetKeyDown(KeyCode.Space) && canPause) TogglePauseWhilstPlaying();
    }
    public void TogglePauseWhilstPlaying()
    {
        if (REF.CombatUI._settingsScript._settingsOn) return;
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
    /// <summary>
    /// Pauses time after some kind of impact, like a projectile hitting its target or a vehicle being eliminated
    /// Effectiveness goes from 0 to 1, 1 being a full time stop
    /// </summary>
    public void TriggerHitStop(float effectiveness, float slowDownDuration, float freezeDuration, float speedUpDuration)
    {
        effectiveness = Mathf.Clamp(effectiveness, 0, 1);

        if(currentHitStop != null) StopCoroutine(currentHitStop);
        currentHitStop = ReturnToRegularTimeScaleFromHitStop(effectiveness, slowDownDuration, freezeDuration, speedUpDuration);
        StartCoroutine(currentHitStop);
    }
    public IEnumerator ReturnToRegularTimeScaleFromHitStop(float effectiveness, float slowDownDuration, float freezeDuration, float speedUpDuration)
    {
        float startScale = 1 - effectiveness;

        float durationCounter = 0;
        while (durationCounter < slowDownDuration)
        {
            durationCounter += Time.fixedDeltaTime;
            Time.timeScale = 1 - ((durationCounter / speedUpDuration) * (1 - startScale));
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        Time.timeScale = startScale;

        yield return new WaitForSecondsRealtime(freezeDuration);

        durationCounter = 0;
        while (durationCounter < speedUpDuration)
        {
            durationCounter += Time.fixedDeltaTime;
            Time.timeScale = startScale + (durationCounter / speedUpDuration) * (1-startScale);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        Time.timeScale = 1;
        Debug.Log("done");
    }
    public void PauseGame()
    {
        paused = true;
        REF.CombatUI._pauseImage.SetActive(paused);
    }
    public void UnpauseGame()
    {
        paused = false;
        REF.CombatUI._pauseImage.SetActive(paused);
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
