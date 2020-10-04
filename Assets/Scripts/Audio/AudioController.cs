using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public enum MUSIC
    {
        MENU,
        GAME
    }

    private const string SFX_VOLUME = "SFXVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string VOLUME = "Volume";
    
    [SerializeField]
    private AudioMixer gameMixer;


    [SerializeField]
    private AudioMixerSnapshot menuMusicSnapshot;
    [SerializeField]
    private AudioMixerSnapshot gameMusicSnapshot;

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //AudioController Functions
    //====================================================================================================================//
    
    public void PlayMusic(MUSIC music)
    {
        switch (music)
        {
            case MUSIC.MENU:
                gameMixer.TransitionToSnapshots(new[] {menuMusicSnapshot, gameMusicSnapshot}, new[] {1f, 0f}, 2f);
                
                break;
            case MUSIC.GAME:
                gameMixer.TransitionToSnapshots(new[] {menuMusicSnapshot, gameMusicSnapshot}, new[] {0f, 1f}, 2f);
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
