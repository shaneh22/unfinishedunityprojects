using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{

    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    public AudioClip chopSound1;
    public AudioClip chopSound2;

    private bool playGameOverSound = true;
    private Animator animator;

    public static bool PlayerMoving = false;

    protected override void Start()
    {
        animator = GetComponent<Animator>();

        foodText.text = "Food: " + GameManager.instance.playerFoodPoints;
        base.Start();
    }
    void Update()
    {
        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }
        if ((horizontal != 0 || vertical != 0) && Input.anyKeyDown && !PlayerMoving)
        {
            AttemptMove<Wall>(horizontal, vertical);
            PlayerMoving = true;
        }
    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.doingSetup)
            {
                return;
            }
        }
        GameManager.instance.playerFoodPoints--;
        foodText.text = "Food: " + GameManager.instance.playerFoodPoints;
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            GameManager.instance.playerMoved++;
        }
        else if (GameManager.instance.swordEquipped && hit.collider != null)
        {
            Transform objectHit = hit.transform;
            if (objectHit.CompareTag("Enemy"))
            {
                animator.SetTrigger("playerChop");
                Invoke("chopSound", float.Epsilon);
                GameObject gameObjectHit = hit.transform.gameObject;
                Enemy enemyHit = gameObjectHit.GetComponent<Enemy>() as Enemy;
                enemyHit.enemyHealth--;
                enemyHit.CheckIfDead();
            }
        }
        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.instance.playerFoodPoints > 0)
        {
            if (collision.tag == "Exit")
            {
                Invoke("Restart", restartLevelDelay);
                enabled = false;
            }
            else if (collision.tag == "Food")
            {
                GameManager.instance.playerFoodPoints += pointsPerFood;
                GameManager.instance.foodEaten++;
                foodText.text = "+" + pointsPerFood + " Food: " + GameManager.instance.playerFoodPoints;
                SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
                collision.gameObject.SetActive(false);
            }
            else if (collision.tag == "Soda")
            {
                GameManager.instance.playerFoodPoints += pointsPerSoda;
                GameManager.instance.foodEaten++;
                foodText.text = "+" + pointsPerSoda + " Food: " + GameManager.instance.playerFoodPoints;
                SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
                collision.gameObject.SetActive(false);
            }
            else if (collision.tag == "Axe")
            {
                GameManager.instance.playerWallDamage=2;
                collision.gameObject.SetActive(false);
            }
            else if (collision.tag == "Sword")
            {
                GameManager.instance.swordEquipped = true;
                collision.gameObject.SetActive(false);
                SoundManager.instance.SwordEquipped();
            }
        }
    }
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(GameManager.instance.playerWallDamage);
        animator.SetTrigger("playerChop");
        Invoke("chopSound", float.Epsilon);
    }

    private void chopSound()
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
    }

    private void Restart()
    {
        if (GameManager.instance.playerFoodPoints > 0)
        {
            SceneManager.LoadScene(0);
            PlayerMoving = false;
        }
    }
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        GameManager.instance.playerFoodPoints -= loss;
        foodText.text = "-" + loss + " Food: " + GameManager.instance.playerFoodPoints;
        CheckIfGameOver();
        GameManager.instance.attacked++;
        GameManager.instance.totalAttacked++;
    }
    public void CheckIfGameOver()
    {
        if (GameManager.instance.playerFoodPoints <= 0)
        {
            GameManager.instance.StopAllCoroutines();
            if (playGameOverSound == true)
            {
                SoundManager.instance.PlaySingle(gameOverSound);
            }
            playGameOverSound = false;
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
    public void RollEyes()
    {
        animator.SetTrigger("playerRollsEyes");
    }
}
