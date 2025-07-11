using System.Collections.Generic;
using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    public string levelName = "Level0";
    public GameObject map;

    public List<StageThreshold> stageThresholds = new List<StageThreshold>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StageManager stageManager = StageManager.instance;

        stageManager.AddLevelStageThresholds(levelName, stageThresholds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
