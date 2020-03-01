using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float moveInput;

    private bool facingRight = true;

    private bool isGrounded;
    private bool wasGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private int jumps = 2; //one jump and an extra jump

    private Rigidbody2D rb;
    private Animator anim;

    public CircleCollider2D cc1;
    public CircleCollider2D cc2;

    public bool cantJump;

    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;

    void Start()
	{
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
	}
    private void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        if (!facingRight && moveInput > 0 || facingRight && moveInput < 0)
        {
            Flip();
        }
        moveInput = 0;
        if (Input.GetKey(right))
        {
            moveInput += 1;
        }
        if (Input.GetKey(left))
        {
            moveInput += -1;
        }

        if (isGrounded && !wasGrounded)
        {
            jumps = 2;
            anim.SetBool("PlayerJumps", false);
            anim.SetBool("PlayerExtraJumps", false);
            anim.SetBool("PlayerFalls", false);
        }

        if (Input.GetKeyDown(up) && jumps > 0 && !cantJump)
        {
            rb.velocity = Vector2.up * jumpForce;
            if (jumps == 2)
            {
                anim.SetBool("PlayerJumps", true);
                anim.SetBool("PlayerFalls", false); //so that it won't transition back to the fall animation;
            }
            else if(jumps == 1)
            {
                anim.SetBool("PlayerExtraJumps", true);
            }
            jumps--;
        }
        else if(Input.GetKeyDown(down)&& !Input.GetKeyDown(up) && isGrounded && wasGrounded)
        {
            cc1.enabled = false;
            cc2.enabled = false;
            anim.SetBool("PlayerFalls", true);
            Invoke("EnableBoxCollider", .38f);
            cantJump = true; //the player can't jump while falling through the ground;
        }

        if(transform.position.y < -7 || Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("VictoryFlag"))
        {
            GameManager.instance.level++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    void EnableBoxCollider()
    {
        cc1.enabled = true;
        cc2.enabled = true;
        cantJump = false; 
    }
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
}
