using UnityEngine;

public class Rush : Patrol
{
    public float rushSpeed;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        RaycastHit2D playerHit = movingRight ? Physics2D.Raycast(groundDetection.position, Vector2.right, 10, LayerMask.GetMask("Player")) : Physics2D.Raycast(groundDetection.position, Vector2.left, 10, LayerMask.GetMask("Player"));
        if (playerHit.collider != null)
        {
            rb.velocity = movingRight ? new Vector2(rushSpeed, rb.velocity.y) : new Vector2(-rushSpeed, rb.velocity.y);
        }
    }
}
