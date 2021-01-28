using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    Rigidbody2D rb;
    public int jumpForce;
    public float jumpTimeInterval;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _ = StartCoroutine(JumpRoutine());
    }

    // Update is called once per frame
    public IEnumerator JumpRoutine()
    {
        while (true)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            yield return new WaitForSeconds(jumpTimeInterval);
        }
    }
}