using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddButtonSFX : MonoBehaviour
{
    private Button _buttonToAddSFXTo;
    private static float pitchVariance;

    private void Awake()
    {
        pitchVariance = 0.1f;

        _buttonToAddSFXTo = GetComponent<Button>();
        if (!_buttonToAddSFXTo) return;

        //on click sfx
        _buttonToAddSFXTo.onClick.AddListener(() => PlayClickSoundEffect());

        //pointer enter sfx
        EventTrigger.Entry pointerEnterEvent = new EventTrigger.Entry();
        pointerEnterEvent.eventID = EventTriggerType.PointerEnter;
        pointerEnterEvent.callback.AddListener((eventData) => { PlayHoverSoundEffect(); });

        _buttonToAddSFXTo.gameObject.AddComponent<EventTrigger>();
        _buttonToAddSFXTo.GetComponent<EventTrigger>().triggers.Add(pointerEnterEvent);
    }

    private void PlayClickSoundEffect()
    {
        if (!REF.AM) return;
        REF.AM._clickedSound.pitch = UnityEngine.Random.Range(1 - pitchVariance, 1 + pitchVariance);
        REF.AM._clickedSound.Play();
    }
    private void PlayHoverSoundEffect()
    {
        if (!REF.AM) return;
        REF.AM._highlightedSound.pitch = UnityEngine.Random.Range(1 - pitchVariance, 1 + pitchVariance);
        REF.AM._highlightedSound.Play();
    }
}
