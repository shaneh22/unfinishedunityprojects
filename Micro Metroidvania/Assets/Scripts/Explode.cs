using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : Enemy
{
    private Animator anim;
    public ParticleSystem explodeParticles;
    public float delayTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    protected override void Die()
    {
        _ = Instantiate(hurtParticles, transform.position, Quaternion.identity);
        anim.SetBool("Flash", true);
        Invoke(nameof(EnemyExplode), delayTime);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
    private void EnemyExplode()
    {
        Vector2 currPos = transform.position;
        _ = Instantiate(coinParticles, currPos + Vector2.up, Quaternion.identity);
        vCam.ScreenShake();
        explodeParticles.Play();
        explodeParticles.transform.parent = null;
        Destroy(gameObject);
        SoundManager.instance.PlaySingle(enemyDead);
    }
}