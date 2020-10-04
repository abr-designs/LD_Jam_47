using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField, TextArea, Header("Background Elements")]
    private string menuPath;

    [SerializeField] private GameObject menuAIRacer;
    [SerializeField] private Transform startPositionTransform;
    [SerializeField] private Transform cameraTransform;

    [SerializeField, Header("UI Elements")]
    private Button playButton;

    [SerializeField] private Button quitButton;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    //Unity Functions
    //====================================================================================================================//

    // Start is called before the first frame update
    private void Start()
    {
        Manager.CameraTransform = cameraTransform;
        SpawnMenuHeat();

        InitUI();
    }


    //MainMenu Functions
    //====================================================================================================================//

    private void InitUI()
    {
        playButton.onClick.AddListener(() =>
        {
            AudioController.Instance.PlayMusic(AudioController.MUSIC.GAME);
            SceneManager.LoadScene(1);
        });

        musicToggle.onValueChanged.AddListener(x => { AudioController.Instance.SetMusicVolume(x ? 0f : -80f); });
        sfxToggle.onValueChanged.AddListener(x => { AudioController.Instance.SetSFXVolume(x ? 0f : -80f); });


#if UNITY_WEBGL && !UNITY_EDITOR
        quitButton.gameObject.SetActive(false);
#else
        quitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();

#endif
        });
#endif


    }

    #region Spawn Background Elements

    private void SpawnMenuHeat()
    {
        var count = 10;
        var recordEvents = JsonConvert.DeserializeObject<List<RecordEvent>>(menuPath);
        for (var i = 0; i < count; i++)
        {
            var ai = SpawnAi(recordEvents, true, i == 0, i);
            ai.invulnerable = true;
            StartCoroutine(Manager.WaitCoroutine(3, () =>
            {
                ai.invulnerable = false;
            }));
        }
    }

    private AIRacer SpawnAi(IReadOnlyList<RecordEvent> recordEvents, bool elastic, bool useAudio, int index)
    {
        var startPos = startPositionTransform.position + (Vector3.right * index);
        var startRotation = startPositionTransform.rotation;
        var temp = Instantiate(menuAIRacer).GetComponentInChildren<AIRacer>();
        temp.useAudio = useAudio;
        if (!useAudio)
            temp.SetTransform(startPos, startRotation);

        temp.PlayBack(new List<RecordEvent>(recordEvents), elastic ? Random.Range(1f, 4f) : 0);

        return temp;
    }

    #endregion //Spawn Background Elements
}
