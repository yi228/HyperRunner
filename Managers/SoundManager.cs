using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    [SerializeField] private AudioClip[] clips;

    void Start()
    {
        instance = this;
        AddClip();
    }
    private void AddClip()
    {
        for(int i = 0; i < clips.Length; i++)
            audioClips.Add(clips[i].name, clips[i]);
    }
    public void PlayClip(AudioSource _audioSource, string _clipKey, bool _loop = false, float _volume = 100f)
    {
        _audioSource.clip = audioClips[_clipKey];
        _audioSource.volume = _volume/100;
        _audioSource.loop = _loop;
        _audioSource.Play();
    }
}
