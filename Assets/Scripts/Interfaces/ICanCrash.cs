using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanCrash
{
    bool invulnerable { get; set; }
    bool isDead { get; }
    float impactForce { get; }
    void OnCollisionEnter(Collision other);
    void Crashed(Collision collision);
}
