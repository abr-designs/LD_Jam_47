using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    public static Action RaceStartedCallback;
    public static Transform CameraTransform;
    
    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject[] obstacles;

    private Vector3 _startLocation;
    private Quaternion _startRotation;

    private bool _startedRace;
    private float _currentLapTime;
    private float _bestLapTime = 9999f;
    private float _totalRaceTime;
    private int _lapCount;
    private int _totalKills;

    private int _totalPoints;

    private List<AIRacer> _opponents;

    private GameUI _gameUI;

    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        _gameUI = FindObjectOfType<GameUI>();
        _opponents = new List<AIRacer>();
        
        _startLocation = player.transform.position;
        _startRotation = player.transform.rotation;
    }

    private void LateUpdate()
    {
        if (player.isDead)
            return;

        if (!_startedRace)
            return;
        
        var deltaTime = Time.deltaTime;
        
        _totalRaceTime += deltaTime;
        _currentLapTime += deltaTime;
        
        _gameUI.SetLapTime(_currentLapTime);
        _gameUI.SetRaceTime(_totalRaceTime);
    }

    private void OnDestroy()
    {
        RaceStartedCallback = null;
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
                ChooseRandomPickup();
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
        _gameUI.SetPowerupText("New Obstacle");
    }

    private void ChooseRandomPickup()
    {
        _gameUI.SetPowerupText("Power-Up");
    }

    private void IncreaseDifficulty()
    {
        
    }

    //====================================================================================================================//

    public void AddPoints(int points)
    {
        _totalPoints += points;
        
        //TODO Update GameUI
        _gameUI.SetPoints(_totalPoints);
    }

    public void AddKill()
    {
        
        _totalKills++;
        _gameUI.SetKills(_totalKills);
        
        AddPoints(1000);
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
            _opponents.TrimExcess();
            
        }
        player.TriggerNewLap();

        if (!_startedRace)
        {
            _startedRace = true;
            RaceStartedCallback?.Invoke();
            return;
        }

        if (_currentLapTime < _bestLapTime)
            _bestLapTime = _currentLapTime;

        _currentLapTime = 0f;
        _lapCount++;
        
        //TODO Need to update GameUI
        _gameUI.SetLapCount(_lapCount);
        _gameUI.SetBestTime(_bestLapTime);
        
        AddPoints(100);

    }

    private void SpawnAi(IReadOnlyList<RecordEvent> recordEvents, bool elastic = false)
    {
        var temp = FactoryManager.Instance.CreateAiRacer();
        temp.SetTransform(_startLocation + Vector3.right * Random.Range(-5, 5), _startRotation);

        temp.PlayBack(new List<RecordEvent>(recordEvents), elastic ? Random.Range(1f, 4f) : 0);

        _opponents.Add(temp);
    }

    //====================================================================================================================//
    
}
