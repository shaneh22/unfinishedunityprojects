using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Camera mainCam;
    private float xPosition;
    private float yPosition;
    public GameObject asteroid;
    public static GameManager instance;
    public int spaceshipNum;
    public GameObject[] spaceships;
    public int score;
    public TMP_Text scoreText;
    public GameObject winScreen;
    public bool spaceshipDead;

    public AudioClip shoot;

    private void Awake()
    {
        instance = this;
        mainCam = FindObjectOfType<Camera>();
        xPosition = mainCam.orthographicSize * 2 * mainCam.aspect;
        yPosition = mainCam.orthographicSize * 2;
        _ = StartCoroutine(SpawnAsteroids());
        winScreen.SetActive(false);
        InstantiateNewSpaceship();
    }

    private void Update()
    {
        //I'm doing it this way because of the multiple different spaceships and multiple guns on said spaceships
        if (Input.GetKeyDown(KeyCode.Space) && !spaceshipDead)
        {
            SoundManager.instance.PlaySingle(shoot);
        }
    }

    private Vector2 GetRandPosition()
    {
        int coinFlip = Random.Range(0, 2);
        float yPos = yPosition;
        if (coinFlip == 0)
        {
            yPos = -yPosition;
        }
        float xPos = Random.Range(-xPosition, xPosition);
        return new Vector2(xPos, yPos);
    }

    private IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            _ = Instantiate(asteroid, GetRandPosition(), Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
            yield return new WaitForSeconds(.5f);
        }
    }

    public void InstantiateNewSpaceship()
    {
        if (spaceshipNum == 13)
        {
            GameWon();
        }
        else
        {
            _ = Instantiate(spaceships[spaceshipNum], transform.position, Quaternion.identity);
            spaceshipDead = false;
        }
    }

    private void GameWon()
    {
        winScreen.SetActive(true);
        _ = StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(1);
            }
            yield return null;
        }
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
