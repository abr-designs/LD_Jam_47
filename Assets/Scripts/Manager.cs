using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private RB_Move player;
    [SerializeField]
    private GameObject aiPrefab;

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

    public void CollectedPowerUp(Powerup.TYPE type)
    {
        ShowRandomObstacle(true);
        
        switch (type)
        {
            case Powerup.TYPE.OBSTACLE:
                ShowRandomObstacle();
                break;
            case Powerup.TYPE.PICKUP:
                break;
            case Powerup.TYPE.DIFFICULTY:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private void ShowRandomObstacle(bool hideAll = false)
    {
        var random = hideAll ? -1 : Random.Range(0, obstacles.Length);
        
        for (var i = 0; i < obstacles.Length; i++)
        {
            if (hideAll)
            {
                obstacles[i].SetActive(false);
                continue;
            }
            
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
