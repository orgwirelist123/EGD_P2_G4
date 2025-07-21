using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource oneShotSource = null;

    public List<AudioContainer> audioContainers;

    protected Dictionary<string, AudioContainer> audioByName = new Dictionary<string, AudioContainer>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        // Initialize the dictionary with all of the audio containers
        foreach (AudioContainer container in audioContainers)
        {
            audioByName.Add(container.name, container);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    AudioContainer GetAudioByName(string name)
    {
        AudioContainer container = audioByName[name];
        if (container == null)
        {
            Debug.LogError(string.Format("Unable to find audio with name {0}", name));
            return null;
        }
        return container;
    }

    public AudioSource PlayAudio(string name, float volume = 1.0f, bool loop = false)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        AudioContainer container = GetAudioByName(name);
        if (container == null) { return null; }

        audioSource.clip = container.audioClip;
        audioSource.loop = loop;
        audioSource.volume = volume * container.defaultVolume;

        return audioSource;
    }

    public void PlayAudioOneShot(string name, float volume)
    {
        AudioContainer container = GetAudioByName(name);
        if (container == null) { return; }

        oneShotSource.PlayOneShot(container.audioClip, volume);
    }
}
