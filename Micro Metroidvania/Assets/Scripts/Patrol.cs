using UnityEngine;

public class Patrol : MonoBehaviour
{
    public float speed;
    public float distance;

    protected bool movingRight = true;

    public Transform groundDetection;

    protected Rigidbody2D rb;

    protected RaycastHit2D groundInfo;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    protected virtual void FixedUpdate()
    {
        groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);
        if (groundInfo.collider == false)
        {
            ChangeDirection();
        }
    }

    protected virtual void ChangeDirection()
    {
        if (movingRight == true)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            Flip();
        }
        else
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            Flip();
        }
    }

    protected virtual void Flip()
    {
        movingRight = !movingRight; //change which way the character is facing
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler; //This will flip the character
    }
}