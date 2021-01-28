using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource musicSource;
    public AudioSource efxSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle(AudioClip clip)
    {
        efxSource.pitch = Random.Range(.95f, 1.05f);
        efxSource.PlayOneShot(clip);
    }
}