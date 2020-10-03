using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Powerup : PickupBase
{
    

    //====================================================================================================================//

    private static Manager _manager;

    //====================================================================================================================//

    [SerializeField]
    private PICKUP type;

    //====================================================================================================================//

    protected override void OnTriggered(Collider other)
    {
        if (!_manager)
            _manager = FindObjectOfType<Manager>();

        _manager.CollectedPowerUp(type);

        Destroy(gameObject);
    }

    //====================================================================================================================//
    
}


