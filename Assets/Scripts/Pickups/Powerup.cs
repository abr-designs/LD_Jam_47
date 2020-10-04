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
    
    //====================================================================================================================//

    private static Manager _manager;

    //====================================================================================================================//

    [SerializeField]
    private PICKUP type;

    private new MeshRenderer renderer => _renderer ?? (_renderer = GetComponent<MeshRenderer>());
    private MeshRenderer _renderer;
    

    //====================================================================================================================//

    private void LateUpdate()
    {
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
    }

    //====================================================================================================================//

    private void SetActive(bool state)
    {
        renderer.enabled = state;
        collider.enabled = state;
        
        coolingDown = !state;
        _t = 0f;
    }
    
}


