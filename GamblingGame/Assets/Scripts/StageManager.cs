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

    public List<Material> skyboxMaterials = new List<Material>();
    public List<float> skyboxThresholds = new List<float>();
    public Material goalSkybox;

    public float moneyCounter = 0;
    public float maxLoadValue = 0;

    public GameObject plinkoBoard;

    public AudioSource rumbleAudio;

    public float goalVolume = 0;

    public GameObject playerCamera;
    public Vector3 baseCameraPosition;

    public float currentXOffset = 0;
    public float maxXOffset = 5;
    public float goalXOffset = 0;

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

        skyboxThresholds.Add(0);
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

        baseCameraPosition = playerCamera.transform.position;

        goalSkybox = RenderSettings.skybox;
    }

    // Update is called once per frame
    void Update()
    {
        goalXOffset = Mathf.Lerp(0, maxXOffset, moneyCounter / maxLoadValue);
        currentXOffset = Mathf.Lerp(currentXOffset, goalXOffset, Time.deltaTime);
        RenderSettings.skybox.Lerp(RenderSettings.skybox, goalSkybox, Time.deltaTime);

        Vector3 cameraOffset = new Vector3(currentXOffset, 0, 0);
        // moneyCounter += Time.deltaTime;
        playerCamera.transform.position = baseCameraPosition + cameraOffset;

        UpdateSkybox();
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

    public void AddSkybox(float maximumLoadValue, Material skyboxMaterial)
    {
        skyboxMaterials.Add(skyboxMaterial);
        skyboxThresholds.Add(maximumLoadValue);
    }

    public void AddMoney(float amount)
    {
        moneyCounter += amount;
    }

    public void UpdateSkybox()
    {
        if (skyboxThresholds.Count == 0 || skyboxMaterials.Count == 0)
        {
            return;
        }

        int indexA = 0;
        int indexB = 1;

        for (int i = 1; i < skyboxThresholds.Count - 2; i++)
        {
            // If we have enough money for this threshold, bump our indices up
            if (skyboxThresholds[i] <= moneyCounter)
            {
                indexA++;
                indexB++;
            }
        }


        // Get the two skyboxes we would be between
        Material skyboxA = skyboxMaterials[indexA];
        Material skyboxB = skyboxMaterials[indexB];

        float threshA = skyboxThresholds[indexA];
        float threshB = skyboxThresholds[indexB];

        float divisor = (threshB - threshA);

        float t = (moneyCounter - threshA) / divisor + 0.01f;

        Debug.Log(string.Format("{0} - {1} - {2} ({3} / {4})", indexA, indexB, t, moneyCounter - threshA, divisor));

        goalSkybox = skyboxA;
        goalSkybox.Lerp(goalSkybox, skyboxB, t);
        //RenderSettings.skybox.Lerp(skyboxA, skyboxB, t);
    }
}
