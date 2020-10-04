using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class Powerup : PickupBase
{
    [SerializeField]
    private float resetTime = 3f;
    private float _t;
    private bool coolingDown;

    private bool disabled;
    
    //====================================================================================================================//

    private static Manager _manager;

    //====================================================================================================================//

    [SerializeField]
    private PICKUP type;

    private new MeshRenderer renderer => _renderer ?? (_renderer = GetComponent<MeshRenderer>());
    private MeshRenderer _renderer;
    

    //====================================================================================================================//

    protected override void Start()
    {
        base.Start();

        Manager.RaceStartedCallback += () =>
        {
            SetActive(true);
            disabled = false;
        };

        //Hide at start until race begins
        disabled = true;
        SetActive(false);
    }

    private void LateUpdate()
    {
        if (disabled)
            return;
        
        if (!coolingDown)
            return;

        if (_t < resetTime)
        {
            _t += Time.deltaTime;
            return;
        }

        SetActive(true);
    }

    protected override void OnTriggered(Collider other)
    {
        if (!_manager)
            _manager = FindObjectOfType<Manager>();

        _manager.CollectedPowerUp(type);

        SetActive(false);

        CreatePowerupAudioEffect();
    }

    //====================================================================================================================//

    private void SetActive(bool state)
    {
        renderer.enabled = state;
        collider.enabled = state;
        
        coolingDown = !state;
        _t = 0f;
    }

    private void CreatePowerupAudioEffect()
    {
        var soundTransform = FactoryManager.Instance.CreatePowerupAudio().transform;
        soundTransform.position = transform.position;
        
        Destroy(soundTransform.gameObject, 2f);
    }
}


