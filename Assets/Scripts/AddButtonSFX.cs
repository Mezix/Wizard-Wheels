using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddButtonSFX : MonoBehaviour
{
    private Button _buttonToAddSFXTo;

    private void Awake()
    {
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
        REF.AM._clickedSound.Play();
    }
    private void PlayHoverSoundEffect()
    {
        REF.AM._highlightedSound.Play();
    }
}
