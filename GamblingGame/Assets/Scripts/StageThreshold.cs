using System.Collections.Generic;
using UnityEngine;

public class StageThreshold : MonoBehaviour
{
    public string thresholdName = "ExampleThreshold";
    public float thresholdLoadValue = 0;
    public float thresholdUnloadValue = 10;
    public float loadDuration = 5;
    public float currentTime = 0;

    private float yOffsetMultiplier = 1.5f;

    public bool loadState = false;

    public Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    public Dictionary<GameObject, Vector3> hiddenPositions = new Dictionary<GameObject, Vector3>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StoreDescendantPositions(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (loadState)
        {
            currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, loadDuration);
        }
        else
        {
            currentTime = Mathf.Clamp(currentTime - Time.deltaTime, 0, loadDuration);
        }

        UpdateObjectPosition();
    }
    private void StoreDescendantPositions(Transform parent)
    {
        foreach (Transform child in parent)
        {
            originalPositions.Add(child.gameObject, child.localPosition);

            Vector3 yOffset = new Vector3(0, child.GetComponent<MeshRenderer>().bounds.size.y, 0);
            Vector3 hiddenPosition = child.localPosition - yOffset * yOffsetMultiplier;
            hiddenPositions.Add(child.gameObject, hiddenPosition);

            child.localPosition = hiddenPosition;

            StoreDescendantPositions(child);
        }
    }

    public void UpdateLoadBasedOnThreshold(float counter)
    {
        if (thresholdLoadValue < counter && counter < thresholdUnloadValue)
        {
            SetLoading(true);
        }
        else
        {
            SetLoading(false);
        }
    }

    public void SetLoading(bool loadState)
    {
        this.loadState = loadState;
    }

    public void UpdateObjectPosition()
    {
        float t = currentTime / loadDuration;
        foreach (GameObject gameObject in originalPositions.Keys)
        {
            Vector3 a = hiddenPositions[gameObject];
            Vector3 b = originalPositions[gameObject];
            Vector3 newPosition = Vector3.Lerp(a, b, t);

            gameObject.transform.localPosition = newPosition;
        }
    }
}
