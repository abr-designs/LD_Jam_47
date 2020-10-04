using System;
using UnityEngine;

public class FactoryManager : MonoBehaviour
{
    public static FactoryManager Instance => _instance;
    private static FactoryManager _instance;
    [SerializeField, Header("Prefabs")]
    private GameObject AiRacerPrefab;
    [SerializeField]
    private GameObject FireEffectPrefab;
    [SerializeField]
    private GameObject ExplosionEffectPrefab;
    [SerializeField]
    private GameObject rocketPrefab;

    [SerializeField, Header("Audio Prefabs")]
    private GameObject rocketExplosionAudioPrefab;
    [SerializeField]
    private GameObject powerupAudioPrefab;
    [SerializeField]
    private GameObject carExplosionAudioPrefab;
    [SerializeField]
    private GameObject carCollisionAudioPrefab;
    //Unity Functions
    //====================================================================================================================//
    
    private void Awake()
    {
        if(_instance != null)
            throw new Exception($"{nameof(FactoryManager)} already exists");

        _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    //====================================================================================================================//
    
    public AIRacer CreateAiRacer()
    {
        return  Instantiate(AiRacerPrefab).GetComponentInChildren<AIRacer>();
    }

    //Abilities
    //====================================================================================================================//

    public Rocket CreateRocket()
    {
        return Instantiate(rocketPrefab).GetComponent<Rocket>();
    }
    
    //Effects
    //====================================================================================================================//
    

    public GameObject CreateFireEffect()
    {
        return Instantiate(FireEffectPrefab);
    }

    public GameObject CreateExplosionEffect()
    {
        return Instantiate(ExplosionEffectPrefab);
    }

    //Audio
    //====================================================================================================================//
    
    public GameObject CreateRocketExplosionAudio()
    {
        return Instantiate(rocketExplosionAudioPrefab);
    }
    public GameObject CreatePowerupAudio()
    {
        return Instantiate(powerupAudioPrefab);
    }
    public GameObject CreateCarExplosionAudio()
    {
        return Instantiate(carExplosionAudioPrefab);
    }
    public GameObject CreateCarCollisionAudio()
    {
        return Instantiate(carCollisionAudioPrefab);
    }
}
