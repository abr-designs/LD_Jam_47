using UnityEngine;

public class StartingLine : MonoBehaviour
{
    [SerializeField]
    private Manager _manager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            _manager.TriggerLap();
    }
}
