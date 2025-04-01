using System.Collections.Generic;
using UnityEngine;

public class SoundEffectLibrary : MonoBehaviour
{
    [SerializeField] private SoundEffectGroup[] soundEffectGroups;
    private Dictionary<string, List<AudioClip>> soundEDictionary;

    void Awake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        soundEDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (SoundEffectGroup soundEffectGroup in soundEffectGroups)
        {
            soundEDictionary[soundEffectGroup.name] = soundEffectGroup.audioClips;
        }
    }

    public AudioClip GetRandomClip(string name) 
    {
        if (soundEDictionary.ContainsKey(name))
        {
            List<AudioClip> clips = soundEDictionary[name];
            if (clips.Count > 0)
            {
                int randomIndex = Random.Range(0, clips.Count);
                return clips[randomIndex];
            }
        }
        return null;
    }
}

[System.Serializable]
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClips;
}
