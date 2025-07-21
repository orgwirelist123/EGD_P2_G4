using System.Collections.Generic;
using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    public string levelName = "Level0";
    public GameObject map;
    public GameObject skyboxPlane;

    public float minimumLoadValue = -1;
    public float maximumUnloadValue = -1;
    public float maximumLoadValue = -1;

    public List<StageThreshold> stageThresholds = new List<StageThreshold>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StageManager stageManager = StageManager.instance;

        stageManager.AddLevelStageThresholds(levelName, stageThresholds);

        foreach (StageThreshold threshold in stageThresholds)
        {
            if (minimumLoadValue == -1 && maximumUnloadValue == -1)
            {
                minimumLoadValue = threshold.thresholdLoadValue;
                maximumUnloadValue = threshold.thresholdUnloadValue;
            }
            else
            {
                minimumLoadValue = Mathf.Min(threshold.thresholdLoadValue, minimumLoadValue);
                maximumUnloadValue = Mathf.Max(threshold.thresholdUnloadValue, maximumUnloadValue);

                maximumLoadValue = Mathf.Max(threshold.thresholdLoadValue, maximumLoadValue);
            }
        }

        stageManager.UpdateMaxLoadValue(maximumLoadValue);
    }

    // Update is called once per frame
    void Update()
    {
        //RenderSettings.skybox.Lerp(skyboxes[0], skyboxes[1], .5f);
        //DynamicGI.UpdateEnvironment();
    }

    void UpdateSkyboxPlaneOpacity()
    {
        if (skyboxPlane == null) { return; }

        float money = StageManager.instance.moneyCounter;
        if (minimumLoadValue < money && money < maximumUnloadValue)
        {
            float duration = maximumUnloadValue - minimumLoadValue;
            float offsetMoney = money - minimumLoadValue;
            float t = offsetMoney / duration;
            float opacity = Mathf.Lerp(1, 0, t);

            Color skyColor = skyboxPlane.GetComponent<MeshRenderer>().material.color;

            skyboxPlane.GetComponent<MeshRenderer>().material.color = new Color(
                skyColor.r,
                skyColor.g,
                skyColor.b,
                opacity
            );
        }
    }
}
