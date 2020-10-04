using System;
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

    //Unity Functions
    //====================================================================================================================//
    
    private void Start()
    {
        ShowEndGameWindow(false);
        
        InitButtons();
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

    public void ShowEndGameWindow(bool state)
    {
        endGameWindow.SetActive(state);
    }
}
