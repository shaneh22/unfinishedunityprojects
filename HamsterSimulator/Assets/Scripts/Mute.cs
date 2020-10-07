using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class Mute : MonoBehaviour
{
    private Animator anim;

    public void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void ToggleMute()
    {
        if (SoundManager.instance.musicSource.mute)
        {
            anim.SetBool("isMuted", true);
        }
        else
        {
            anim.SetBool("isMuted", false);
        }
    }
}
