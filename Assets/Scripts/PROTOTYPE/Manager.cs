using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    public static Transform CameraTransform;
    
    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject[] obstacles;

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

    public void CollectedPowerUp(PICKUP type)
    {
        switch (type)
        {
            case PICKUP.OBSTACLE:
                ShowRandomObstacle();
                break;
            case PICKUP.PICKUP:
                break;
            case PICKUP.DIFFICULTY:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private void ShowRandomObstacle()
    {
        var random = Random.Range(-1, obstacles.Length);
        
        for (var i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].SetActive(i == random);
        }
    }

    private void ChooseRandomPickup()
    {
        
    }

    private void IncreaseDifficulty()
    {
        
    }

    //====================================================================================================================//
    

    public void TriggerLap()
    {
        var recordEvents = player.RecordEvents;

        if (recordEvents.Count > 0)
        {
            var count = false/*Random.value > 0.5 */? 1 : Random.Range(2, 5);
            for (var i = 0; i < count; i++)
            {
                SpawnAi(recordEvents, count > 1);
            }
        }
        
        player.TriggerNewLap();
    }

    private void SpawnAi(IReadOnlyList<RecordEvent> recordEvents, bool elastic = false)
    {
        var temp = FactoryManager.Instance.CreateAiRacer();
        temp.SetTransform(_startLocation + Vector3.right * Random.Range(-5, 5), _startRotation);

        temp.PlayBack(new List<RecordEvent>(recordEvents), elastic ? Random.Range(1f, 4f) : 0);
    }

    //====================================================================================================================//
    
}
