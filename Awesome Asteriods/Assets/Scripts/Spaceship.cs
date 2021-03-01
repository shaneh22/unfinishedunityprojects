using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    private Rigidbody2D rb;
    public float thrust;
    //public float rotationSpeed;
    public float maxSpeed;
    public GameObject engine;
    private Camera mainCam;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = FindObjectOfType<Camera>();
        anim = GetComponent<Animator>();
        engine.SetActive(false);
    }
    private void Update()
    {
        ControlEngine();
    }
    private void FixedUpdate()
    {
        ControlSpaceship();
        CheckPosition();
    }
    private void ControlSpaceship()
    {
        if (!GameManager.instance.spaceshipDead)
        {
            //Pretty sure transform.rotate works badly with rigid bodies
            //transform.Rotate(0, 0, -Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
            if(Input.GetAxis("Horizontal") != 0)
            {
                rb.freezeRotation = false;
                rb.AddTorque(-Input.GetAxis("Horizontal") * Time.deltaTime * 25);
            }
            else
            {
                rb.freezeRotation = true;
            }
            rb.AddForce(transform.up * thrust * Input.GetAxis("Vertical"));
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed));
        }
    }
    private void ControlEngine()
    {
        if (Input.GetAxis("Vertical") > 0 && !GameManager.instance.spaceshipDead)
        {
            engine.SetActive(true);
            SoundManager.instance.thrustSource.Play();
        }
        else
        {
            engine.SetActive(false);
            SoundManager.instance.thrustSource.Stop();
        }
    }
    private void CheckPosition()
    {

        float sceneWidth = mainCam.orthographicSize * 2 * mainCam.aspect;
        float sceneHeight = mainCam.orthographicSize * 2;

        float sceneRightEdge = sceneWidth / 2;
        float sceneLeftEdge = sceneRightEdge * -1;
        float sceneTopEdge = sceneHeight / 2;
        float sceneBottomEdge = sceneTopEdge * -1;

        if (transform.position.x > sceneRightEdge)
        {
            transform.position = new Vector2(sceneLeftEdge, transform.position.y);
        }
        if (transform.position.x < sceneLeftEdge) { transform.position = new Vector2(sceneRightEdge, transform.position.y); }
        if (transform.position.y > sceneTopEdge)
        {
            transform.position = new Vector2(transform.position.x, sceneBottomEdge);
        }
        if (transform.position.y < sceneBottomEdge)
        {
            transform.position = new Vector2(transform.position.x, sceneTopEdge);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        anim.SetTrigger("Destroy");
        GameManager.instance.spaceshipDead = true;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = true;
    }
    public void DestroySpaceship()
    {
        Destroy(gameObject);
        GameManager.instance.spaceshipNum++;
        GameManager.instance.InstantiateNewSpaceship();
    }
}

//Using the tutorial from https://oxmond.com/asteroids-arcade-gameplay/ 