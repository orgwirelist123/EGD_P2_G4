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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadAdditionalLevel(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
    }

    public void AddLevelStageThresholds(string levelName, List<StageThreshold> stageThresholds)
    {
        this.stageThresholds.AddRange(stageThresholds);
    }
}
