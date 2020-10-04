using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    public static Action RaceStartedCallback;
    public static Transform CameraTransform;

    [SerializeField] private Transform startPositionTransform;
    
    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject[] obstacles;

    [SerializeField, TextArea] 
    private string jsonPath;

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
        
        AudioController.Instance.PlayMusic(AudioController.MUSIC.GAME);

        RaceStartedCallback += TriggerLap;
        
        SpawnInitialHeat();
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

    private int lastObstacle;
    private ABILITY lastAbility;

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
        int random = Random.Range(0, obstacles.Length);
        

        while (random == lastObstacle)
        {
            random = Random.Range(0, obstacles.Length);
        }
        
        
        for (var i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].SetActive(i == random);
        }
        _gameUI.SetPowerupText("New Obstacle");
    }

    private void ChooseRandomPickup()
    {
        var max = Enum.GetValues(typeof(ABILITY)).Length;
        ABILITY chosenAbility = (ABILITY) Random.Range(1, max);

        while (chosenAbility == lastAbility)
        {
            chosenAbility = (ABILITY) Random.Range(1, max);
        }

        lastAbility = chosenAbility;
        
        player.SetAbility(chosenAbility);
        _gameUI.SetPowerupText(chosenAbility.GetAbilityName());
    }

    /*private void IncreaseDifficulty()
    {
        
    }*/

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

        if (recordEvents.Count > 10)
        {
            var count = Random.Range(4, 6);
            for (var i = 0; i < count; i++)
            {
                var ai = SpawnAi(recordEvents, true, i);
                //Avoid accidenttally killing themselves
                ai.invulnerable = true;
                StartCoroutine(WaitCoroutine(1f, () =>
                {
                    ai.invulnerable = false;
                }));
            }
            _opponents.TrimExcess();
            
        }
        player.TriggerNewLap();

        if (!_startedRace)
        {
            _startedRace = true;
            //RaceStartedCallback?.Invoke();
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
    
    private void SpawnInitialHeat()
    {
        var count = 5;
        var recordEvents = JsonConvert.DeserializeObject<List<RecordEvent>>(jsonPath);
        for (var i = 0; i < count; i++)
        {
            var waitForStart = SpawnAi(recordEvents, true, i);
            
            waitForStart._replaying = false;
            waitForStart.invulnerable = true;
            
            RaceStartedCallback += () =>
            {
                waitForStart._replaying = true;
                waitForStart.invulnerable = false;
            };
        }
    }

    private AIRacer SpawnAi(IReadOnlyList<RecordEvent> recordEvents, bool elastic, int index)
    {
        var startPos = startPositionTransform.position + (Vector3.right * index);
        var temp = FactoryManager.Instance.CreateAiRacer();
        temp.SetTransform(startPos, _startRotation);

        temp.PlayBack(new List<RecordEvent>(recordEvents), elastic ? Random.Range(1f, 4f) : 0);

        _opponents.Add(temp);

        return temp;
    }

    //====================================================================================================================//

    public static IEnumerator WaitCoroutine(float time, Action Callback)
    {
        yield return new WaitForSeconds(time);
        
        Callback?.Invoke();

        Callback = null;
    }
    
}
