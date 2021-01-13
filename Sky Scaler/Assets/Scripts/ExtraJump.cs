using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    private Animator anim;
    public void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void Collision()
    {
        anim.SetTrigger("Collide");
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
        Invoke(nameof(Reset), 2f);
    }
    private void Reset()
    {
        gameObject.SetActive(true);
    }
}
