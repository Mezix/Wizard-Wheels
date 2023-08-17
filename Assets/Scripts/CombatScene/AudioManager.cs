using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;

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

    //  Everloop

    public enum BGMusicMode
    {
        Combat,
        NonCombat,
        Menu
    }
    public EverloopController _everloop;
    public List<AudioSource> _combatSources = new List<AudioSource>();
    public List<AudioSource> _nonCombatSources = new List<AudioSource>();
    public List<AudioSource> _menuSources = new List<AudioSource>();
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
    private void Start()
    {
        PlayMusic(Loader.SceneToSceneType(SceneManager.GetActiveScene()));
    }

    public void PlayMusic(Loader.SceneType scene)
    {
        if (scene.ToString() == "CombatScene")
        {
            PlayMusic(BGMusicMode.Combat);
        }
        if (scene.ToString() == "MenuScene")
        {
            PlayMusic(BGMusicMode.Menu);
        }
        if (scene.ToString() == "EventScene")
        {
            PlayMusic(BGMusicMode.NonCombat);
        }
        if (scene.ToString() == "RouteTransitionScene")
        {
            PlayMusic(BGMusicMode.NonCombat);
        }
        if (scene.ToString() == "ConstructionScene")
        {
            PlayMusic(BGMusicMode.NonCombat);
        }
    }
    void PlayMusic(BGMusicMode mode)
    {
        float fadeIn = 50f; //amount in Fixedupdate frames
        float fadeOut = 20f; //amount in Fixedupdate frames
        bool ignoreTimeScale = false;
        if (mode.Equals(BGMusicMode.Combat))
        {
            foreach (AudioSource src in _combatSources) if (!src.isPlaying) _everloop.FadeInLayer(src, fadeIn, ignoreTimeScale);

            foreach (AudioSource src in _nonCombatSources) if (src.isPlaying) _everloop.FadeOutLayer(src, fadeOut, ignoreTimeScale);
            foreach (AudioSource src in _menuSources) if (src.isPlaying) _everloop.FadeOutLayer(src, fadeOut, ignoreTimeScale);
        }
        else if (mode.Equals(BGMusicMode.NonCombat))
        {
            foreach (AudioSource src in _nonCombatSources) if (!src.isPlaying) _everloop.FadeInLayer(src, fadeIn, ignoreTimeScale);

            foreach (AudioSource src in _combatSources) if (src.isPlaying) _everloop.FadeOutLayer(src, fadeOut, ignoreTimeScale);
            foreach (AudioSource src in _menuSources) if (src.isPlaying) _everloop.FadeOutLayer(src, fadeOut, ignoreTimeScale);
        }
        else if (mode.Equals(BGMusicMode.Menu))
        {
            foreach (AudioSource src in _menuSources) if (!src.isPlaying) _everloop.FadeInLayer(src, fadeIn, ignoreTimeScale);

            foreach (AudioSource src in _combatSources) if (src.isPlaying) _everloop.FadeOutLayer(src, fadeOut, ignoreTimeScale);
            foreach (AudioSource src in _nonCombatSources) if (src.isPlaying) _everloop.FadeOutLayer(src, fadeOut, ignoreTimeScale);
        }
        else
        {
            _everloop.FadeOutAll();
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
