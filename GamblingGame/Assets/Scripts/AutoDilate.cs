using TMPro;
using UnityEngine;

public class AutoDilate : MonoBehaviour
{
    public TextMeshPro text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float value = (1.0f/2) * Mathf.Sin( Time.realtimeSinceStartup );
        value = (1.0f/2) * Mathf.Sin(Time.realtimeSinceStartup);
        //Debug.Log(value);
        text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, value);
    }
}
