using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-1000)]
public class AudioController : MonoBehaviour
{
    public enum MUSIC
    {
        MENU,
        GAME
    }

    public static AudioController Instance => _instance;
    private static AudioController _instance;

    private const string SFX_VOLUME = "SFXVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string VOLUME = "Volume";
    
    
    [SerializeField]
    private AudioMixer gameMixer;
    [SerializeField]
    private AudioMixer musicMixer;


    [SerializeField]
    private AudioMixerSnapshot menuMusicSnapshot;
    [SerializeField]
    private AudioMixerSnapshot gameMusicSnapshot;

    //Unity Functions
    //====================================================================================================================//

    private void Awake()
    {
        if(_instance != null)
            Destroy(gameObject);

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        PlayMusic(MUSIC.MENU);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
    //AudioController Functions
    //====================================================================================================================//
    
    public void PlayMusic(MUSIC music)
    {
        switch (music)
        {
            case MUSIC.MENU:
                musicMixer.TransitionToSnapshots(new[] {menuMusicSnapshot, gameMusicSnapshot}, new[] {1f, 0f}, 2f);
                
                break;
            case MUSIC.GAME:
                musicMixer.TransitionToSnapshots(new[] {menuMusicSnapshot, gameMusicSnapshot}, new[] {0f, 1f}, 2f);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(music), music, null);
        }
    }

    public void SetVolume(float volume)
    {
        gameMixer.SetFloat(VOLUME, volume);
    }

    public void SetSFXVolume(float volume)
    {
        gameMixer.SetFloat(SFX_VOLUME, volume);
    }

    public void SetMusicVolume(float volume)
    {
        gameMixer.SetFloat(MUSIC_VOLUME, volume);
    }
}
