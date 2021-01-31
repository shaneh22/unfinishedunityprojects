using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public ParticleSystem deathParticles;
    public ParticleSystem hurtParticles;
    private bool invincible;
    public float invincibilityTime;

    public AudioClip enemyHurt;
    public AudioClip enemyDead;

    public VCam vCam;

    public GameObject coinParticles;

    private void Start()
    {
        vCam = FindObjectOfType<VCam>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Sword"))
        {
            LoseHealth(Player.instance.playerStrength);
        }
    }
    private void LoseHealth(int damage)
    {
        if (!invincible)
        {
            vCam.ScreenShake();
            SoundManager.instance.PlaySingle(enemyHurt);
            invincible = true;
            Invoke(nameof(StopInvincible), invincibilityTime);
            health -= damage;
            if (!CheckIfDead())
            {
                _ = Instantiate(hurtParticles, transform.position, Quaternion.identity);
            }
        }
    }
    private bool CheckIfDead()
    {
        if (health <= 0)
        {
            Die();
            return true;
        }
        else
        {
            return false;
        }
    }

    protected virtual void Die()
    {
        vCam.ScreenShake();
        SoundManager.instance.PlaySingle(enemyDead);
        Vector2 currPos = transform.position;
        _ = Instantiate(deathParticles, currPos, Quaternion.identity);
        _ = Instantiate(coinParticles, currPos + Vector2.up, Quaternion.identity);
        Destroy(gameObject);
    }

    private void StopInvincible()
    {
        invincible = false;
    }

}
