using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;

    private Animator animator;
    private Transform target;
    public bool skipMove = true;
    public bool hasMoved;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    public int enemyHealth = 1;
    // Start is called before the first frame update
    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        hasMoved = true;
        if (GameManager.instance.level >= 28 && GameManager.instance.swordEquipped)
        {
            skipMove = false;
        }
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);
        if (GameManager.instance.level < 28 || !GameManager.instance.swordEquipped)
        {
            skipMove = true;
        }
    }
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;
        AttemptMove<Player>(xDir, yDir);
        if (blocked && GameManager.instance.level >= 7)
        {
            yDir = target.position.y >= transform.position.y ? 1 : -1;
            if (!Move(0, yDir, out RaycastHit2D hit))
            {
                xDir = target.position.x > transform.position.x ? 1 : -1;
                Move(xDir, 0, out RaycastHit2D raycastHit);
            }
        }
    }
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
        hitPlayer.LoseFood(playerDamage);
    }
    public void CheckIfDead()
    {
        if (enemyHealth <= 0)
        {
            GameManager.instance.enemies.Remove(this);
            gameObject.SetActive(false);
            GameManager.instance.monstersKilled++;
        }
    }
}
