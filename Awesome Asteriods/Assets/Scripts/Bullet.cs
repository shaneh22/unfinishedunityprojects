using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletRange;
    private Rigidbody2D rb;
    private void Awake()
    {
        transform.parent = null;
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * bulletSpeed);
        Invoke(nameof(DestroyBullet), bulletRange);
    }
    private void OnCollisionEnter(Collision collision)
    {
        DestroyBullet();
    }
    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
