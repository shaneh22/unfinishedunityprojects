using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float moveInput;

    private Rigidbody rb;

    void Start()
	{
        rb = GetComponent<RigidBody2D>();
	}
}
