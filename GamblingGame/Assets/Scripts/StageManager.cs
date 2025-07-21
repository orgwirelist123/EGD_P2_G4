using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    protected List<StageThreshold> stageThresholds = new List<StageThreshold>();

    public List<string> levelNames = new List<string>();

    public float moneyCounter = 0;
    public float maxLoadValue = 0;

    public GameObject plinkoBoard;

    public AudioSource rumbleAudio;

    public float goalVolume = 0;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this);
                return;
            }
        }
        else
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (string levelName in levelNames)
        {
            LoadAdditionalLevel(levelName);
        }

        rumbleAudio = AudioManager.instance.PlayAudio("Rumble", 1.0f, true);
        rumbleAudio.Play();
        //rumbleAudio.Pause();
    }

    // Update is called once per frame
    void Update()
    {

        // moneyCounter += Time.deltaTime;
    }

    public void UpdateMaxLoadValue(float newMaxLoadValue)
    {
        maxLoadValue = Mathf.Max(newMaxLoadValue, maxLoadValue);
    }

    private void FixedUpdate()
    {
        CheckStageThresholds();
        rumbleAudio.volume = Mathf.Lerp(rumbleAudio.volume, goalVolume, Time.fixedDeltaTime);
    }

    public void LoadAdditionalLevel(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
    }

    public void AddLevelStageThresholds(string levelName, List<StageThreshold> stageThresholds)
    {
        this.stageThresholds.AddRange(stageThresholds);
    }

    public void CheckStageThresholds()
    {
        bool anyMoving = false;
        foreach (StageThreshold threshold in stageThresholds)
        {
            threshold.UpdateLoadBasedOnThreshold(moneyCounter);
            anyMoving = anyMoving || threshold.stillMoving;
        }

        Debug.Log(anyMoving);
        if (anyMoving)
        {
            // If any of them are still moving, then unpause the audio
            //rumbleAudio.UnPause();
            goalVolume = 1.0f;
        }
        else if (!anyMoving)
        {
            // If none of them are moving, pause the audio
            //rumbleAudio.Pause();
            goalVolume = 0f;
        }
    }

    public void AddMoney(float amount)
    {
        moneyCounter += amount;
    }
}
