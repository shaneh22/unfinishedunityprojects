using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int level;

    [SerializeField] private AudioClip[] musicTracks;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    //This is called each time a scene is loaded.
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        level = SceneManager.GetActiveScene().buildIndex + 1;
        ChooseMusic();
    }

    void OnEnable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void OnDisable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
        //Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void ChooseMusic()
    {
        if(level <= 10)
        {
            CheckIfPlaying(musicTracks[0]);
        }
        else if(level <= 20)
        {
            CheckIfPlaying(musicTracks[1]);
        }
        else if(level <= 30)
        {
            CheckIfPlaying(musicTracks[2]);
        }
        else if(level <= 40)
        {
            CheckIfPlaying(musicTracks[3]);
        }
        else if(level <= 49)
        {
            CheckIfPlaying(musicTracks[4]);
        }
        else if(level == 50)
        {
             //Level 50 is special 
        }
    }

    private void CheckIfPlaying(AudioClip music)
    {
        if (SoundManager.instance.musicSource.clip != music)
        {
            SoundManager.instance.musicSource.Stop();
            SoundManager.instance.musicSource.clip = music;
            SoundManager.instance.musicSource.Play();
        }
    }
}
