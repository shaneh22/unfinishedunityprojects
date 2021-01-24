using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed; //How fast the player moves left and right
    public float jumpForce; //How high the player can jump
    private float moveInput;
    private Rigidbody2D rb;

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

    public int numCoins; //Could be displayed using UI, not really important

    private bool isDashing; //used because you can't jump while you dash
    private float dashTimeCounter;
    public float dashTime; //how long does the dash last
    public float dashSpeed; //how fast the dash makes the player go
    private int dashDirection; //keep same direction during dash
    // Start is called before the first frame update
    PlayerControls controls;

    float move;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jumps = numJumps; //set the number of extra jumps, currently at 1. (double jump)
        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<float>();
        controls.Gameplay.Move.canceled += ctx => move = 0;

        controls.Gameplay.Jump.performed += ctx => Jump();
        controls.Gameplay.Jump.canceled += ctx => CancelJump();

        controls.Gameplay.Dash.performed += ctx => Dash();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }
    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void Jump()
    {
        if (wallSliding) //If they are wallSliding and jump, they do a wall jump
        {
            wallJumping = true;
            wallJumpDirection = -moveInput;
            Flip(); //The character was facing the wall, now they are jumping off it in the reverse direction
            Invoke(nameof(SetWallJumpingFalse), wallJumpTime);
        }
        else if (jumps > 0 && !isDashing) //Jump 
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); //Multiply jumpForce to make player jump
            if (isGrounded)
            {
                isJumping = true;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * .75f);  //give a little more force to the jump in the air
                jumps--; //Decrease jumps 
            }
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
            ResetJumps();
            if(dashTimeCounter == 0)
            {
                dashTimeCounter = -1;
                Invoke(nameof(ResetDash), 1f); //Delay when the player can dash again
            }
        }

        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround); //for wallJump, is the player touching a wall 
        wallSliding = isTouchingFront == true && isGrounded == false && moveInput != 0; //player is sliding on a wall if they are moving against one and not on ground
        if (wallSliding)
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
    }

    void Dash()
    {
        if (dashTimeCounter > 0)
        {
            isDashing = true;
            dashDirection = facingLeft ? -1 : 1; //set dash direction based on how the character is facing
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
        if (collision.gameObject.CompareTag("ExtraJump")) //When the player collides with a object tagged 'Extra Jump', they get an extra jump
        {
            collision.gameObject.GetComponent<ExtraJump>().Collision(); //This will disable the object temporarily
            ResetJumps(); //Give the player another jump
        }
        else if (collision.gameObject.CompareTag("Spikes"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //If the player hits spikes, restart level.
        }
        else if (collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject); //When the player collects a coin, destroy it.
            numCoins++;
        }
        else if (collision.gameObject.CompareTag("ExtraDash"))
        {
            collision.gameObject.GetComponent<ExtraJump>().Collision(); //Same as ExtraJump because we just want the object to disappear for a few seconds
            if (isDashing)
            {
                Invoke(nameof(ResetDash), dashTimeCounter + .1f); //If they are dashing, we want to reset the dash after they finish
            }
            else
            {
                ResetDash(); //gives another dash for while in the air or on cooldown on ground
            }
        }
    }

    void Flip()
    {
        facingLeft = !facingLeft; //change which way the character is facing
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler; //This will flip the character
    }
}
