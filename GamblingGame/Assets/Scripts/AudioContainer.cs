using UnityEngine;

[System.Serializable]
public class AudioContainer
{
    [SerializeField] public string name = "TemplateName";
    [SerializeField] public AudioClip audioClip = null;
    [SerializeField] public float defaultVolume = 1.0f;
}
