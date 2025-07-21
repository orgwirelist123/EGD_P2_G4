using UnityEngine;

public class MoneyDisplayController : MonoBehaviour
{
    public GameObject barFront;

    public float targetRatio = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        targetRatio = Mathf.Clamp01(StageManager.instance.moneyCounter / StageManager.instance.maxLoadValue);

        RectTransform barTransform = barFront.GetComponent<RectTransform>();

        float t = Time.deltaTime;
        float a = barTransform.localScale.y;
        float b = targetRatio;

        barTransform.localScale = new Vector3(1, Mathf.Lerp(a, b, t), 1);
    }
}
