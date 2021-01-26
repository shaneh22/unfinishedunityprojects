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
            Vector2 currPos = transform.position;
            _ = Instantiate(deathParticles, currPos, Quaternion.identity);
            Destroy(gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }
    private void StopInvincible()
    {
        invincible = false;
    }
}
