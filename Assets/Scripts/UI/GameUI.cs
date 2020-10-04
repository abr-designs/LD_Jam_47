using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class GameUI : MonoBehaviour
{
    [SerializeField]
    private int gameSceneIndex = 0;
    [SerializeField]
    private int menuSceneIndex;
    
    [SerializeField, Header("End Game Screen")]
    private GameObject endGameWindow;
    [SerializeField]
    private TMP_Text descriptionText;
    [SerializeField]
    private Button restartButton;

    [SerializeField, Header("Lap Texts")] 
    private TMP_Text lapText;
    [SerializeField]
    private TMP_Text raceTimeText;
    [SerializeField]
    private TMP_Text lapTimeText;
    [SerializeField]
    private TMP_Text bestTimeText;
    
    [SerializeField, Header("Other Texts")] 
    private TMP_Text pointsText;
    [SerializeField]
    private TMP_Text killsText;
    [SerializeField]
    private TMP_Text powerUpText;

    [SerializeField, Header("Countdown")] 
    private GameObject countDownWindow;
    [SerializeField]
    private Image[] lights;

    private IEnumerator fade;
    
    //Unity Functions
    //====================================================================================================================//
    
    private void Start()
    {
        InitUI();
    }
    

    //GameUI Functions
    //====================================================================================================================//
    
    private void InitButtons()
    {
        restartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(gameSceneIndex);
        });
    }

    //====================================================================================================================//

    private void InitUI()
    {
        ShowEndGameWindow(false);
        InitButtons();

        
        SetLapCount(0);
        SetRaceTime(0f);
        SetLapTime(0f);
        SetBestTime(0f);
        SetPowerupText(string.Empty);
        SetPoints(0);
        SetKills(0);

        countDownWindow.SetActive(true);
        StartCoroutine(CountdownCoroutine(0.5f));

    }
    public void SetLapCount(int laps)
    {
        lapText.text = $"Lap {laps}";
    }

    public void SetRaceTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);

        raceTimeText.text = $"Total {time:m\\:ss}:{time.Milliseconds:00}";
    }

    public void SetLapTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);

        lapTimeText.text = $"Time {time:m\\:ss}:{time.Milliseconds:00}";
    }

    public void SetBestTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);

        bestTimeText.text = $"Best {time:m\\:ss}:{time.Milliseconds:00}";
    }

    public void SetPoints(int points)
    {
        pointsText.text = $"Points {points}";
    }

    public void SetKills(int kills)
    {
        killsText.text = $"Kills {kills}";
    }

    public void SetPowerupText(string powerup)
    {
        powerUpText.text = $"{powerup}";

        if (string.IsNullOrEmpty(powerup))
            return;

        if (fade != null)
        {
            powerUpText.color = _startColor;
            StopCoroutine(fade);
        }

        StartCoroutine(FadeCoroutine(2));
    }
    
    public void ShowEndGameWindow(bool state)
    {
        endGameWindow.SetActive(state);
        descriptionText.text = string.Join("\n", new []
        {
            raceTimeText.text,
            bestTimeText.text,
            pointsText.text,
            killsText.text
        });
    }

    //====================================================================================================================//

    private Color _startColor;
    private IEnumerator FadeCoroutine(float waitSeconds)
    {
        _startColor = powerUpText.color;
        var endColor = _startColor;
        endColor.a = 0f;

        var t = 0f;
        
        yield return new WaitForSeconds(waitSeconds);

        while (t < 1f)
        {
            powerUpText.color = Color.Lerp(_startColor, endColor, t);

            t += Time.deltaTime;

            yield return null;
        }

        powerUpText.text = string.Empty;
        powerUpText.color = _startColor;

        fade = null;
    }

    private IEnumerator CountdownCoroutine(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        
        for (var i = 0; i < lights.Length; i++)
        {
            if (i == lights.Length - 1)
            {
                lights[i].color = Color.green;
                
                yield return new WaitForSeconds(0.2f);
                
                countDownWindow.SetActive(false);
                break;
            }
            else
            {
                lights[i].color = Color.red;
            }
            
            yield return new WaitForSeconds(delaySeconds);
        }
        
        Manager.RaceStartedCallback?.Invoke();
    }
    
}
