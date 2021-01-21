using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    private Animator anim;
    private CircleCollider2D cc;

    public float RespawnTime = 2f;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        cc = GetComponent<CircleCollider2D>();
    }
    public void Collision()
    {
        anim.SetTrigger("Collide");
        cc.enabled = false;
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
        Invoke(nameof(Reset), RespawnTime);
    }
    private void Reset()
    {
        gameObject.SetActive(true);
        cc.enabled = true;
    }
}
