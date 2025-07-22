using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public Image blackScreenImage;

    public float goalScreenOpacity = 0;

    public GameObject personPrefab;

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
        UpdateBlackScreen();
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

        int indexA = -1;
        int indexB = 0;

        for (int i = 0; i < skyboxThresholds.Count - 1; i++)
        {
            // If we have enough money for this threshold, bump our indices up
            if (skyboxThresholds[i] <= moneyCounter)
            {
                indexA++;
                indexB++;
            }
        }

        indexA = Mathf.Clamp(indexA, 0, skyboxMaterials.Count-1);
        indexB = Mathf.Clamp(indexB, 0, skyboxMaterials.Count-1);

        // Get the two skyboxes we would be between
        Material skyboxA = skyboxMaterials[indexA];
        Material skyboxB = skyboxMaterials[indexB];

        float threshA = skyboxThresholds[indexA];
        float threshB = skyboxThresholds[indexB];

        float divisor = (threshB - threshA);

        float t = (moneyCounter - threshA) / divisor + 0.01f;

        //Debug.Log(string.Format("{0} - {1} - {2} ({3} / {4})", indexA, indexB, t, moneyCounter - threshA, divisor));

        goalSkybox = skyboxA;
        goalSkybox.Lerp(goalSkybox, skyboxB, t);
        //RenderSettings.skybox.Lerp(skyboxA, skyboxB, t);
    }

    public void ExecutePlayer()
    {
        blackScreenImage.color = new Color(blackScreenImage.color.r, blackScreenImage.color.g, blackScreenImage.color.b, 1);
        goalScreenOpacity = 1;
        Invoke("DelaySetOpacity", 3f);

        GameObject person = Instantiate(personPrefab, playerCamera.transform);
        person.transform.SetParent(gameObject.transform);

        person.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-10.0f, -200.0f), Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f)));
    }

    public void DelaySetOpacity()
    {
        goalScreenOpacity = 0;
        PlinkoController.instance.canAct = true;
    }

    public void UpdateBlackScreen()
    {
        float a = blackScreenImage.color.a;
        float b = goalScreenOpacity;
        float t = Time.deltaTime;
        blackScreenImage.color = new Color(blackScreenImage.color.r, blackScreenImage.color.g, blackScreenImage.color.b, Mathf.Lerp(a, b, t));
    }
}
