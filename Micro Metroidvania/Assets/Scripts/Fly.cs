using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    Rigidbody2D rb;
    public float swoop;
    public float speed;
    public float distance;
    public Vector2 initialPosition;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
    }

    public void OnEnable()
    {
        _ = StartCoroutine(SwoopCoroutine());
        _ = StartCoroutine(MoveCoroutine());
        transform.position = initialPosition;
    }

    public IEnumerator SwoopCoroutine()
    {
        while (true)
        {
            rb.velocity = new Vector2(rb.velocity.x, swoop);
            yield return new WaitForSeconds(1f);
            rb.velocity = new Vector2(rb.velocity.x, -swoop);
            yield return new WaitForSeconds(1f);
        }
    }
    public IEnumerator MoveCoroutine()
    {
        while (true)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            yield return new WaitForSeconds(distance);
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            yield return new WaitForSeconds(distance);
        }
    }
}
