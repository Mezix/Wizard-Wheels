using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Singleton;
    public AudioMixer audioMixer;

    //  Audio Sources

    public AudioSource _highlightedSound;
    public AudioSource _clickedSound;

    //  Player Prefs

    public const string MasterVolumePrefString  = "MasterVol";
    public const string MusicVolumePrefString  = "MusicVol";
    public const string SFXVolumePrefString  = "SFXVol";

    public float masterVolume;
    public float musicVolume;
    public float SFXVolume;
    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerPrefs();
        }
    }

    private void LoadPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(MasterVolumePrefString)) PlayerPrefs.SetFloat(MasterVolumePrefString, masterVolume);
        if (!PlayerPrefs.HasKey(MusicVolumePrefString)) PlayerPrefs.SetFloat(MusicVolumePrefString, musicVolume);
        if (!PlayerPrefs.HasKey(SFXVolumePrefString)) PlayerPrefs.SetFloat(SFXVolumePrefString, SFXVolume);

        masterVolume = PlayerPrefs.GetFloat(MasterVolumePrefString);
        musicVolume = PlayerPrefs.GetFloat(MusicVolumePrefString);
        SFXVolume = PlayerPrefs.GetFloat(SFXVolumePrefString);

        SaveVolumeSettingsToPlayerPrefs();
    }

    public void SaveVolumeSettingsToPlayerPrefs()
    {
        PlayerPrefs.SetFloat(MasterVolumePrefString, masterVolume);
        PlayerPrefs.SetFloat(MusicVolumePrefString, musicVolume);
        PlayerPrefs.SetFloat(SFXVolumePrefString, SFXVolume);
        PlayerPrefs.Save();
    }
}
