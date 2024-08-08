using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField] SoundDatabase soundDatabase;
    [SerializeField] int sourceLength;
    List<AudioSource> audioSources = new List<AudioSource>();
    List<SoundName> currentFramePlaySounds = new List<SoundName>();

    private void Start()
    {
        for (int i = 0; i < sourceLength; i++)
        {
            audioSources.Add(gameObject.AddComponent<AudioSource>());
        }
    }

    private void Update()
    {
        currentFramePlaySounds.Clear();
    }


    public void Play(SoundName soundName)
    {
        //同じフレームに同じクリップを再生しようとしていればキャンセル
        foreach (SoundName checkName in currentFramePlaySounds)
        {
            if (checkName == soundName) return;
        }
        currentFramePlaySounds.Add(soundName);


        AudioClip audioClip = soundDatabase.soundDatas.Find(s => s.name == soundName.ToString()).audioClip;

        


        AudioSource audioSource = GetAudioSource();
        if (audioSource == null)
        {
            Debug.LogError("SoundManager AudioSource is insufficient");
            return;
        }
        
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private AudioSource GetAudioSource()
    {
        foreach(AudioSource audioSource in audioSources)
        {
            if (!audioSource.isPlaying) return audioSource;
        }
        return null;
    }
    
}

