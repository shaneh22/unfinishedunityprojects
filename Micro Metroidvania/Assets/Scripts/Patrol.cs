using UnityEngine;

public class Patrol : MonoBehaviour
{
    public float speed;
    public float distance;

    private bool movingRight = true;

    public Transform groundDetection;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    private void FixedUpdate()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);
        if (groundInfo.collider == false)
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
    }

    void Flip()
    {
        movingRight = !movingRight; //change which way the character is facing
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler; //This will flip the character
    }
}