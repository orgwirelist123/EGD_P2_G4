using System.Collections.Generic;
using UnityEngine;

public class StageThreshold : MonoBehaviour
{
    public string thresholdName = "ExampleThreshold";
    public float thresholdLoadValue = 0;
    public float thresholdUnloadValue = 10;
    public float loadSpeed = 1;

    public float loadDuration = 5;
    public float unloadMultiplier = 1;

    public float currentTime = 0;

    private float yOffsetMultiplier = 1.5f;

    public bool loadState = false;
    public bool stillMoving = false;

    public Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    public Dictionary<GameObject, Vector3> hiddenPositions = new Dictionary<GameObject, Vector3>();
    public Dictionary<GameObject, Vector3> targetPositions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StoreDescendantPositions(transform);
    }

    // Update is called once per frame
    void Update()
    {
        float beforeTime = currentTime;
        if (loadState)
        {
            currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, loadDuration);
            targetPositions = originalPositions;
        }
        else
        {
            currentTime = Mathf.Clamp(currentTime - Time.deltaTime * unloadMultiplier, 0, loadDuration);
            targetPositions = hiddenPositions;
        }

        // If it was clamped and remains the same, then we've reached one of the two ends
        if (currentTime == beforeTime)
        {
            stillMoving = false;
        }
        else
        {
            stillMoving = true;
        }

        UpdateObjectPosition();
    }
    private void StoreDescendantPositions(Transform parent)
    {
        float mapY = parent.parent.parent.transform.position.y;

        foreach (Transform child in parent)
        {
            originalPositions.Add(child.gameObject, child.localPosition);

            Bounds combinedBounds = new Bounds(Vector3.zero, Vector3.zero);
            var renderers = child.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer render in renderers)
            {
                if (combinedBounds.size.magnitude == 0)
                {
                    combinedBounds = render.bounds;
                }
                else
                {
                    combinedBounds.Encapsulate(render.bounds);
                }
            }

            Vector3 yOffset = new Vector3(0, combinedBounds.size.y, 0);
            Vector3 positionYOffset = new Vector3(0, Mathf.Abs(child.localPosition.y) + mapY, 0);
            Vector3 hiddenPosition = child.localPosition - (yOffset * yOffsetMultiplier) - (positionYOffset);
            hiddenPositions.Add(child.gameObject, hiddenPosition);

            child.localPosition = hiddenPosition;

            //StoreDescendantPositions(child);
        }
    }

    public void UpdateLoadBasedOnThreshold(float counter)
    {
        if (thresholdLoadValue <= counter && counter <= thresholdUnloadValue)
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
        /*
        float t = currentTime / loadDuration;
        foreach (GameObject gameObject in originalPositions.Keys)
        {
            Vector3 a = hiddenPositions[gameObject];
            Vector3 b = originalPositions[gameObject];
            Vector3 newPosition = Vector3.Lerp(a, b, t);

            gameObject.transform.localPosition = newPosition;
        }
        */

        foreach (GameObject gameObject in originalPositions.Keys)
        {
            float t = Time.deltaTime;
            Vector3 a = gameObject.transform.localPosition;
            Vector3 b = targetPositions[gameObject];

            gameObject.transform.localPosition = Vector3.Lerp(a, b, t);
        }
    }
}
