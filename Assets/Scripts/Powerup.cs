using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : PickupBase
{
    public enum TYPE
    {
        OBSTACLE,
        PICKUP,
        DIFFICULTY
    }
    protected override void OnTriggered(Collider other)
    {
        Destroy(gameObject);
    }
}


