using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System;

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
    public bool facingLeft;

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
    public bool vengeanceEnabled;

    PlayerControls controls;

    float move;

    public static Player instance;
    public int lives = 3;
    private int maxLives = 3;
//    public float lifeForce;

    private Vector2 savespotPosition = new Vector2(-3, 14);
    public ParticleSystem savedGame;
    public ParticleSystem deathParticles;
    public ParticleSystem dust;
    public ParticleSystem blueDust;

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

    public TMP_Text coinsText;
    public ParticleSystem coinsChanged;

    private float jumpPressedRememberTime;
    private float wasGroundedTime;

    public AudioClip dashSound;
    public AudioClip blessing;
    public AudioClip deathSound;
    public AudioClip healSound;
    public AudioClip hurtSound;
    public AudioClip jumpSound;
    public AudioClip swordSound;
    public AudioClip coinSound;
    public AudioClip coinsSound;

    public int numCoins;
    public GameObject coinParticles;

    public VCam vCam;

    private Vector2 lastGroundedPosition;

    private int numDeaths;
    private DateTime startingTime;
    private DateTime endingTime;
    private string speedrunTime;
    public GameObject winScreen;

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

        vCam = FindObjectOfType<VCam>();
        controls.Gameplay.LookDown.performed += ctx => LookDown();
        controls.Gameplay.LookDown.canceled += ctx => ResetCamera();

        healthText = GameObject.Find("HealthText").GetComponent<TMP_Text>();
        lifeChanged = GameObject.Find("LifeChanged").GetComponent<ParticleSystem>();

        coinsText = GameObject.Find("CoinsText").GetComponent<TMP_Text>();
        coinsChanged = GameObject.Find("CoinChanged").GetComponent<ParticleSystem>();

        healthText.text = maxLives + "";

        _ = StartCoroutine(LastGroundedPosition());

        startingTime = DateTime.Now;

        winScreen = GameObject.Find("WinScreen");
        winScreen.SetActive(false);
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
        coinsText = GameObject.Find("CoinsText").GetComponent<TMP_Text>();
        coinsChanged = GameObject.Find("CoinChanged").GetComponent<ParticleSystem>();
        vCam = FindObjectOfType<VCam>();
        SetHealthText();
        _ = StartCoroutine(LastGroundedPosition());
        winScreen = GameObject.Find("WinScreen");
        winScreen.SetActive(false);
    }

    private IEnumerator LastGroundedPosition()
    {
        while (true)
        {
            if (isGrounded)
            {
                lastGroundedPosition = transform.position;
                yield return new WaitForSeconds(2f);
            }
            yield return null;
        }
    }

    private void LookDown()
    {
        if (isGrounded)
        {
            vCam.LookDown();
        }
    }

    private void ResetCamera()
    {
        vCam.ResetCameraPosition();
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
            dust.Play();
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
                dust.Play();
                SoundManager.instance.PlaySingle(jumpSound);
                ResetDash();
                wasGroundedTime = 0;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); //Multiply jumpForce to make player jump
                isJumping = true;
            }
            else if (doubleJumpEnabled)
            {
                blueDust.Play();
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
                dust.Play();
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
            if (isGrounded)
            {
                dust.Play();
            }
            else
            {
                blueDust.Play();
            }
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
            playerStrength = initialStrength;
        }
        else if (collision.gameObject.CompareTag("PowerUp"))
        {
            SoundManager.instance.PlaySingle(blessing);
            SoundManager.instance.musicSource.Pause();
            collision.gameObject.GetComponent<PowerUp>().Collison();
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
        else if (collision.gameObject.CompareTag("Coin"))
        {
            SoundManager.instance.PlaySingle(coinSound);
            _ = Instantiate(coinParticles, collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseLife();
        }
        else if (collision.gameObject.CompareTag("Spikes"))
        {
            LoseLife();
            if (lives > 0)
            {
                controls.Disable();
                rb.velocity = Vector2.zero;
                transform.position = lastGroundedPosition;
                controls.Enable();
            }
        }
    }

    private void OnParticleCollision(GameObject collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            LoseLife();
        }
        else if (collision.CompareTag("Coin"))
        {
            numCoins++;
            SetCoinsText();
            SoundManager.instance.PlaySingle(coinsSound);
            List<ParticleCollisionEvent> events;
            events = new List<ParticleCollisionEvent>();

            ParticleSystem m_System = collision.GetComponent<ParticleSystem>();

            ParticleSystem.Particle[] m_Particles;
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];

            ParticlePhysicsExtensions.GetCollisionEvents(collision.GetComponent<ParticleSystem>(), gameObject, events);
            foreach (ParticleCollisionEvent coll in events)
            {
                if (coll.intersection != Vector3.zero)
                {
                    int numParticlesAlive = m_System.GetParticles(m_Particles);

                    // Check only the particles that are alive
                    for (int i = 0; i < numParticlesAlive; i++)
                    {

                        //If the collision was close enough to the particle position, destroy it
                        if (Vector3.Magnitude(m_Particles[i].position - coll.intersection) < 0.05f)
                        {
                            m_Particles[i].remainingLifetime = -1; //Kills the particle
                            m_System.SetParticles(m_Particles); // Update particle system
                            break;
                        }
                    }
                }
            }
        }
    }

    void Flip()
    {
        if(isGrounded) dust.Play();
        facingLeft = !facingLeft; //change which way the character is facing
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler; //This will flip the character
        sword.SetActive(false);
    }

    public void LoseLife()
    {
        if (!invincible)
        {
            anim.SetBool("Invincible", true);
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
            numDeaths++;
            numCoins = 0;
            SetCoinsText();
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
        vCam.ScreenShake();
        controls.Gameplay.Disable();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        savedGame.Play();
        Invoke(nameof(ResetGravity), 2f);
    }

    public void ResetGravity()
    {
        SoundManager.instance.musicSource.UnPause();
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

    private void SetCoinsText()
    {
        if (coinsText != null && coinsChanged != null)
        {
            coinsText.text = numCoins + "";
            coinsChanged.Play();
        }
    }

    public void BossDefeated()
    {
        Invoke(nameof(GameWon), 3f);
    }

    private void GameWon()
    {
        winScreen.SetActive(true);
        controls.Disable();
        GameObject.Find("Deaths").GetComponent<TMP_Text>().text = "Deaths: " + numDeaths;
        TMP_Text speedrunText = GameObject.Find("Time").GetComponent<TMP_Text>();
        endingTime = DateTime.Now;
        int speedrunHour = endingTime.Hour < startingTime.Hour ? 24 + endingTime.Hour - startingTime.Hour : endingTime.Hour - startingTime.Hour;
        int speedrunMinute = endingTime.Minute < startingTime.Minute ? 60 + endingTime.Minute - startingTime.Minute : endingTime.Minute - startingTime.Minute;
        int speedrunSecond = endingTime.Second < startingTime.Second ? 60 + endingTime.Second - startingTime.Second : endingTime.Second - startingTime.Second;
        speedrunTime = LeadingZero(speedrunHour) + ":" + LeadingZero(speedrunMinute) + ":" + LeadingZero(speedrunSecond);
        speedrunText.text = "Time: " + speedrunTime;
        GameObject.Find("Grade").GetComponent<TMP_Text>().text = "Grade: " + Grade();
    }

    private string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

    private string Grade()
    {
        if (maxLives == 5 && numCoins >= 300)
        {
            return "S";
        }
        else if (maxLives == 5 && numCoins >= 200)
        {
            return "A";
        }
        else if (maxLives == 5 && numCoins >= 100)
        {
            return "B";
        }
        else if ((maxLives == 5 && numCoins >= 50) || numCoins >= 100)
        {
            return "C";
        }
        else if(maxLives >= 4)
        {
            return "D";
        }
        else
        {
            return "F";
        }
    }

}
