using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
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
    public void DestroyCoin()
    {
        Destroy(gameObject);
    }
}
