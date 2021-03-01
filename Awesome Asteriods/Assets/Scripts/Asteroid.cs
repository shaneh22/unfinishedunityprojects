using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rb;
    private float xPosition;
    private float yPosition;
    private Camera mainCam;
    public int id;
    public GameObject asteroid;
    public GameObject smallerAsteroid;
    public GameObject smallestAsteroid;
    public float invincibleTime = .2f;
    public AudioClip explode;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = FindObjectOfType<Camera>();
        rb.AddForce(transform.up * 100);
        xPosition = mainCam.orthographicSize * 2 * mainCam.aspect;
        yPosition = mainCam.orthographicSize * 2;
        _ = StartCoroutine(CheckIfOnScreen());
        rb.AddTorque(30);
    }

    private void Update()
    {
        if(invincibleTime > 0)
        {
            invincibleTime -= Time.deltaTime;
        }
    }

    private IEnumerator CheckIfOnScreen()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (transform.position.x > xPosition || transform.position.x < -xPosition || transform.position.y > yPosition || transform.position.y < -yPosition)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Asteroid"))
        {
            SplitAsteroid();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SplitAsteroid();
    }
    private void SplitAsteroid()
    {
        if(invincibleTime > 0)
        {
            return;
        }
        SoundManager.instance.PlaySingle(explode);
        gameObject.SetActive(false);
        switch (id)
        {
            case 0:
                _ = Instantiate(asteroid, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
                _ = Instantiate(smallerAsteroid, transform.position + new Vector3(2, 2), Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
                GameManager.instance.score += 20;
                break;
            case 1:
                _ = Instantiate(smallestAsteroid, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
                GameManager.instance.score += 50;
                break;
            case 2:
                _ = Instantiate(smallestAsteroid, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
                GameManager.instance.score += 70;
                break;
            case 3:
                GameManager.instance.score += 100;
                break;
        }
        GameManager.instance.UpdateScoreText();
        Destroy(gameObject);
    }
}
