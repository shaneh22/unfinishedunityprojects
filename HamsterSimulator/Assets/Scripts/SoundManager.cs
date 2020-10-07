using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource efxSource;

    public static SoundManager instance;

    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySingle(AudioClip clip)
    {
        efxSource.PlayOneShot(clip);
    }
    public void ToggleMute()
    {
        musicSource.mute = !musicSource.mute;
    }
}
