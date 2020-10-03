using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private RB_Move player;
    [SerializeField]
    private GameObject aiPrefab;

    private Vector3 _startLocation;
    private Quaternion _startRotation;

    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        _startLocation = player.transform.position;
        _startRotation = player.transform.rotation;
    }

    //====================================================================================================================//

    public void CollectedPowerUp(Powerup.TYPE type)
    {
        
    }

    public void TriggerLap()
    {
        var inputEvents = player.InputEvents;
        
        if(inputEvents.Count > 0)
            SpawnAi(inputEvents);
        
        player.TriggerNewLap();
    }

    private void SpawnAi(IReadOnlyList<RB_Move.InputEvent> inputEvents)
    {
        var temp = Instantiate(aiPrefab).GetComponentInChildren<Test_AI>();
        temp.transform.position = _startLocation;
        temp.transform.rotation = _startRotation;
        
        temp.PlayBack(new List<RB_Move.InputEvent>(inputEvents));
    }

    //====================================================================================================================//
    
}
