using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddUIElementsSFX : MonoBehaviour
{ 
    private Button _buttonToAddSFXTo;
    private Toggle _toggleToAddSFXTo;
    private static float pitchVariance;

    private void Awake()
    {
        pitchVariance = 0.1f;

        //pointer enter sfx
        EventTrigger.Entry pointerEnterEvent = new EventTrigger.Entry();
        pointerEnterEvent.eventID = EventTriggerType.PointerEnter;
        pointerEnterEvent.callback.AddListener((eventData) => { PlayHoverSoundEffect(); });

        _buttonToAddSFXTo = GetComponent<Button>();
        if (_buttonToAddSFXTo)
        {
            //on click sfx
            _buttonToAddSFXTo.onClick.AddListener(() => PlayClickSoundEffect());


            _buttonToAddSFXTo.gameObject.AddComponent<EventTrigger>();
            _buttonToAddSFXTo.GetComponent<EventTrigger>().triggers.Add(pointerEnterEvent);
        }
        _toggleToAddSFXTo = GetComponent<Toggle>();
        if(_toggleToAddSFXTo)
        {
            _toggleToAddSFXTo.onValueChanged.AddListener( delegate { PlayClickSoundEffect(); });

            _toggleToAddSFXTo.gameObject.AddComponent<EventTrigger>();
            _toggleToAddSFXTo.GetComponent<EventTrigger>().triggers.Add(pointerEnterEvent);
        }
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
