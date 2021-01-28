using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class Player : MonoBehaviour
{
    public float speed; //How fast the player moves left and right
    public float jumpForce; //How high the player can jump
    private float moveInput;
    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    public int numJumps = 1;
    private int jumps;
    private bool facingLeft;

    bool isTouchingFront;
    public Transform frontCheck;
    bool wallSliding;
    public float wallSlidingSpeed;
    private bool wallJumping;
    public float wallJumpTime;
    private float wallJumpDirection;

    public float fallMultiplier = 2.5f;

    private bool isJumping;

    private bool isDashing; //used because you can't jump while you dash
    private float dashTimeCounter;
    public float dashTime; //how long does the dash last
    public float dashSpeed; //how fast the dash makes the player go
    private int dashDirection; //keep same direction during dash
    // Start is called before the first frame update

//    public bool healEnabled;
//    public bool blastEnabled;
    public bool dashEnabled;
    public bool doubleJumpEnabled;
    public bool wallJumpEnabled;

    PlayerControls controls;

    float move;

    public static Player instance;
    public int lives = 3;
    private int maxLives = 3;
//    public float lifeForce;

    private Vector2 savespotPosition = new Vector2(-3, 14);
    public ParticleSystem savedGame;
    public ParticleSystem deathParticles;

    [SerializeField] private float invincibleTime;
    private bool invincible;

    public int initialStrength;
    public int playerStrength;
    public GameObject sword;

    public float attackSpeed;
    public float attackDelayTime;
    private float attackDelay;

    public TMP_Text healthText;
    public ParticleSystem lifeChanged;

    private float jumpPressedRememberTime;
    private float wasGroundedTime;

    public AudioClip dashSound;
    public AudioClip blessing;
    public AudioClip deathSound;
    public AudioClip healSound;
    public AudioClip hurtSound;
    public AudioClip jumpSound;
    public AudioClip swordSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sword.SetActive(false);

        jumps = numJumps; //set the number of extra jumps, currently at 1. (double jump)
        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<float>();
        controls.Gameplay.Move.canceled += ctx => move = 0;

        controls.Gameplay.Jump.performed += ctx => Jump();
        controls.Gameplay.Jump.canceled += ctx => CancelJump();

        controls.Gameplay.Dash.performed += ctx => Dash();

        controls.Gameplay.Attack.performed += ctx => Attack();

        healthText = GameObject.Find("HealthText").GetComponent<TMP_Text>();
        lifeChanged = GameObject.Find("LifeChanged").GetComponent<ParticleSystem>();
        healthText.text = maxLives + "";
    }

    private void OnEnable()
    {
        playerStrength = initialStrength;
        lives = maxLives;
        controls.Gameplay.Enable();
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        InitGame();
    }

    private void InitGame()
    {
        healthText = GameObject.Find("HealthText").GetComponent<TMP_Text>();
        lifeChanged = GameObject.Find("LifeChanged").GetComponent<ParticleSystem>();
        SetHealthText();
    }

    private void Attack()
    {
        if (attackDelay <= 0)
        {
            SoundManager.instance.PlaySingle(swordSound);
            attackDelay = attackDelayTime;
            sword.SetActive(true);
            Invoke(nameof(StopAttack), attackSpeed);
        }
    }

    private void StopAttack()
    {
        sword.SetActive(false);
    }

    private void Jump()
    {
        if (wallSliding && wallJumpEnabled) //If they are wallSliding and jump, they do a wall jump
        {
            SoundManager.instance.PlaySingle(jumpSound);
            wallJumping = true;
            wallJumpDirection = -moveInput;
            Flip(); //The character was facing the wall, now they are jumping off it in the reverse direction
            Invoke(nameof(SetWallJumpingFalse), wallJumpTime);
        }
        else if (jumps > 0 && !isDashing) //Jump 
        {
            if (isGrounded || wasGroundedTime > 0)
            {
                SoundManager.instance.PlaySingle(jumpSound);
                ResetDash();
                wasGroundedTime = 0;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); //Multiply jumpForce to make player jump
                isJumping = true;
            }
            else if (doubleJumpEnabled)
            {
                SoundManager.instance.PlaySingle(jumpSound);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * .75f);  //give a little more force to the jump in the air
                jumps--; //Decrease jumps 
            }
        }
        else
        {
            jumpPressedRememberTime = .2f;
        }
    }

    private void CancelJump()
    {
        if (isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
            isJumping = false;
        }
    }

    private void Update()
    {
        if (isGrounded) //If the player is on the ground, reset the number of jumps they have.
        {
            wasGroundedTime = .2f;
            ResetJumps();
            if(dashTimeCounter == 0)
            {
                dashTimeCounter = -1;
                Invoke(nameof(ResetDash), .25f); ; //Delay when the player can dash again
            }
            if (jumpPressedRememberTime > 0 && !isDashing)
            {
                SoundManager.instance.PlaySingle(jumpSound);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); //Multiply jumpForce to make player jump
                isJumping = true;
                jumpPressedRememberTime = 0;
            }
            else
            {
                jumpPressedRememberTime -= Time.deltaTime;
            }
        }
        else
        {
            wasGroundedTime -= Time.deltaTime;
        }

        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround); //for wallJump, is the player touching a wall 
        wallSliding = isTouchingFront == true && isGrounded == false && moveInput != 0; //player is sliding on a wall if they are moving against one and not on ground
        if (wallSliding && wallJumpEnabled)
        {
            //If they are wallSliding, reduce their velocity downwards
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        if (wallJumping)
        {
            rb.velocity = new Vector2(speed * wallJumpDirection, jumpForce * .75f); //jump up and in reverse direction away from wall
        }
        if(rb.velocity.y < 0) //Better looking jump: More gravity on the way down than up
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            isJumping = false;
        }
        if (isDashing) //and I'm not walking about the player's looks ;)
        {
            if(dashTimeCounter > 0)
            {
                rb.velocity = new Vector2(dashSpeed * dashDirection, 0); // y is 0 so that the y position stays the same
                dashTimeCounter -= Time.deltaTime;
            }
            else
            {
                isDashing = false;
                dashTimeCounter = 0;
            }
        }
        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
        }
    }

    void Dash()
    {
        if (dashTimeCounter > 0 && dashEnabled)
        {
            isDashing = true;
            dashDirection = facingLeft ? -1 : 1; //set dash direction based on how the character is facing
            SoundManager.instance.PlaySingle(dashSound);
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        //moveInput = move.x > 0 ? 1 : move.x < 0 ? -1 : 0; //movement is flipped
        moveInput = move;
        if (!wallJumping && !isDashing) //In order to make player jump away from wall, player control of left/right movement must be disabled (also can't change movement during dash)
        {
            if ((!facingLeft && moveInput < 0) || (facingLeft && moveInput > 0))
            {
                Flip(); //Make sure our character is facing the right way
            }
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y); //move left or right based on moveInput
        }
    }

    private void SetWallJumpingFalse()
    {
        wallJumping = false; //when this is set to false, the player regains control of their character's left and right movement
        ResetJumps(); //Personally, I think they should get their jump back after being on a wall.
    }

    private void ResetJumps()
    {
        jumps = numJumps;
    }

    private void ResetDash()
    {
        dashTimeCounter = dashTime; //Reset the dashTimeCounter so the player can dash again
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Save"))
        {
            SoundManager.instance.PlaySingle(healSound);
            lives = maxLives;
            SetHealthText();
            savespotPosition = transform.position;
            savedGame.Play();
            collision.gameObject.GetComponent<Disappear>().Collision();
        }
        else if (collision.gameObject.CompareTag("PowerUp"))
        {
            SoundManager.instance.PlaySingle(blessing);
            Destroy(collision.gameObject);
            PowerUp();
        }
        else if (collision.gameObject.CompareTag("Life"))
        {
            SoundManager.instance.PlaySingle(healSound);
            maxLives++;
            lives = maxLives;
            SetHealthText();
            PowerUp();
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseLife();
            anim.SetBool("Invincible", true);
        }
        else if (collision.gameObject.CompareTag("Spikes"))
        {
            lives = 0;
            CheckIfDead(); //If the player hits spikes, restart level.
        }
    }

    private void OnParticleCollision(GameObject collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            LoseLife();
            anim.SetBool("Invincible", true);
        }
    }

    void Flip()
    {
        facingLeft = !facingLeft; //change which way the character is facing
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler; //This will flip the character
    }

    public void LoseLife()
    {
        if (!invincible)
        {
            SoundManager.instance.PlaySingle(hurtSound);
            playerStrength += 2;
            lives--;
            SetHealthText();
            CheckIfDead();
            invincible = true;
            Invoke(nameof(StopInvincibility), invincibleTime);
        }
    }

    public void StopInvincibility()
    {
        invincible = false;
        anim.SetBool("Invincible", false);
    }

    public void CheckIfDead()
    {
        if (lives <= 0)
        {
            SoundManager.instance.PlaySingle(deathSound);
            Vector2 currPos = transform.position;
            _ = Instantiate(deathParticles, currPos, Quaternion.identity);
            gameObject.SetActive(false);
            Invoke(nameof(RestartToSavedLocation), 1f);
        }
    }

    public void RestartToSavedLocation()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        transform.position = savespotPosition;
        gameObject.SetActive(true);
    }

    public void PowerUp()
    {
        controls.Gameplay.Disable();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        savedGame.Play();
        Invoke(nameof(ResetGravity), 1f);
    }

    public void ResetGravity()
    {
        rb.gravityScale = 1;
        controls.Gameplay.Enable();
    }

    private void SetHealthText()
    {
        if (healthText != null && lifeChanged != null)
        {
            healthText.text = lives + "";
            lifeChanged.Play();
        }
    }
}
