using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class PickupBase : MonoBehaviour
{
    [SerializeField]
    private string compareTag;
    
    protected new Collider collider;
    
    // Start is called before the first frame update
    private void Start()
    {
        collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(compareTag))
            return;

        OnTriggered(other);
    }

    protected abstract void OnTriggered(Collider other);
}

