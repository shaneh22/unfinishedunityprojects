using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    Rigidbody2D rb;
    public float bounce;
    public Vector2 initialPosition;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
    }

    public void OnEnable()
    {
        _ = StartCoroutine(BounceCoroutine());
        transform.position = initialPosition;
    }

    public IEnumerator BounceCoroutine()
    {
        while (true)
        {
            rb.velocity = Vector2.up * bounce;
            yield return new WaitForSeconds(.5f);
            rb.velocity = -Vector2.up * bounce;
            yield return new WaitForSeconds(.5f);
        }
    }
}
