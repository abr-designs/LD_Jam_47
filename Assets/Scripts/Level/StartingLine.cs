using UnityEngine;

public class StartingLine : MonoBehaviour
{
    [SerializeField]
    private Manager _manager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        var velocity = other.attachedRigidbody.velocity.normalized;

        if (Vector3.Dot(velocity, transform.forward.normalized) <= 0f)
            return;
        
        
        _manager.TriggerLap();
    }
}
