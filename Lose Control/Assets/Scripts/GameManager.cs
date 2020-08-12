using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int level = 1;
    public static GameManager instance = null;
    public PlayerController pc;

    public TMP_Text upText;
    public TMP_Text leftText;
    public TMP_Text downText;
    public TMP_Text rightText;
    public TMP_Text victoryText;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); 
    }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Call InitGame to initialize our level.
        InitGame();
    }
    void OnEnable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void OnDisable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
        //Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    public void InitGame()
    {
        StopAllCoroutines();
        pc = FindObjectOfType<PlayerController>();
        upText = GameObject.Find("UpText").GetComponent<TMP_Text>();
        leftText = GameObject.Find("LeftText").GetComponent<TMP_Text>();
        downText = GameObject.Find("DownText").GetComponent<TMP_Text>();
        rightText = GameObject.Find("RightText").GetComponent<TMP_Text>();
        victoryText = GameObject.Find("VictoryText").GetComponent<TMP_Text>();
        victoryText.gameObject.SetActive(false);
        _ = StartCoroutine(ChangeControls());
    }
    private IEnumerator ChangeControls()
    {
        switch (level)
        {
            case 1:
                pc.up = KeyCode.W;
                upText.text = "W";

                pc.left = KeyCode.A;
                leftText.text = "A";

                pc.down = KeyCode.S;
                downText.text = "S";

                pc.right = KeyCode.D;
                rightText.text = "D";
                break;
            case 2:
                pc.up = KeyCode.S;
                upText.text = "S";

                pc.left = KeyCode.D;
                leftText.text = "D";

                pc.down = KeyCode.W;
                downText.text = "W";

                pc.right = KeyCode.A;
                rightText.text = "A";
                break;
            case 3:
                pc.up = KeyCode.D;
                upText.text = "D";

                pc.left = KeyCode.W;
                leftText.text = "W";

                pc.down = KeyCode.A;
                downText.text = "A";

                pc.right = KeyCode.S;
                rightText.text = "S";
                break;
            case 4:
                while (level == 4)
                {
                    pc.up = KeyCode.W;
                    upText.text = "W";

                    pc.left = KeyCode.A;
                    leftText.text = "A";

                    pc.down = KeyCode.S;
                    downText.text = "S";

                    pc.right = KeyCode.D;
                    rightText.text = "D";
                    yield return new WaitForSecondsRealtime(3f);
                    pc.up = KeyCode.S;
                    upText.text = "S";

                    pc.left = KeyCode.D;
                    leftText.text = "D";

                    pc.down = KeyCode.W;
                    downText.text = "W";

                    pc.right = KeyCode.A;
                    rightText.text = "A";
                    yield return new WaitForSecondsRealtime(3f);
                }
                break;
            case 5:
                pc.up = KeyCode.A;
                upText.text = "A";

                pc.left = KeyCode.D;
                leftText.text = "D";

                pc.down = KeyCode.Q;
                downText.text = "Q";

                pc.right = KeyCode.E;
                rightText.text = "E";
                break;
            case 6:
                pc.up = KeyCode.E;
                upText.text = "E";

                pc.left = KeyCode.A;
                leftText.text = "A";

                pc.down = KeyCode.T;
                downText.text = "T";

                pc.right = KeyCode.J;
                rightText.text = "J";
                break;
            case 7:
                pc.up = KeyCode.Alpha1;
                upText.text = "1";

                pc.left = KeyCode.Alpha2;
                leftText.text = "2";

                pc.down = KeyCode.Alpha3;
                downText.text = "3";

                pc.right = KeyCode.Alpha4;
                rightText.text = "4";
                break;
            case 8:
                pc.up = KeyCode.T;
                upText.text = "T";

                pc.left = KeyCode.Z;
                leftText.text = "Z";

                pc.down = KeyCode.V;
                downText.text = "V";

                pc.right = KeyCode.M;
                rightText.text = "M";
                break;
            case 9:
                while (level == 9)
                {
                    pc.up = KeyCode.W;
                    upText.text = "W";

                    pc.left = KeyCode.A;
                    leftText.text = "A";

                    pc.down = KeyCode.S;
                    downText.text = "S";

                    pc.right = KeyCode.D;
                    rightText.text = "D";
                    yield return new WaitForSeconds(3f);
                    pc.up = KeyCode.E;
                    upText.text = "E";

                    pc.left = KeyCode.S;
                    leftText.text = "S";

                    pc.down = KeyCode.D;
                    downText.text = "D";

                    pc.right = KeyCode.F;
                    rightText.text = "F";
                    yield return new WaitForSeconds(3f);
                    pc.up = KeyCode.T;
                    upText.text = "T";

                    pc.left = KeyCode.F;
                    leftText.text = "F";

                    pc.down = KeyCode.G;
                    downText.text = "G";

                    pc.right = KeyCode.H;
                    rightText.text = "H";
                    yield return new WaitForSeconds(3f);
                    pc.up = KeyCode.Y;
                    upText.text = "Y";

                    pc.left = KeyCode.G;
                    leftText.text = "G";

                    pc.down = KeyCode.H;
                    downText.text = "H";

                    pc.right = KeyCode.J;
                    rightText.text = "J";
                    yield return new WaitForSeconds(3f);
                    pc.up = KeyCode.U;
                    upText.text = "U";

                    pc.left = KeyCode.H;
                    leftText.text = "H";

                    pc.down = KeyCode.J;
                    downText.text = "J";

                    pc.right = KeyCode.K;
                    rightText.text = "K";
                    yield return new WaitForSeconds(3f);
                    pc.up = KeyCode.I;
                    upText.text = "I";

                    pc.left = KeyCode.J;
                    leftText.text = "J";

                    pc.down = KeyCode.K;
                    downText.text = "K";

                    pc.right = KeyCode.L;
                    rightText.text = "L";
                    yield return new WaitForSeconds(3f);
                }
                break;
            case 10:
                pc.up = KeyCode.Space;
                upText.text = "⎵";

                pc.left = KeyCode.A;
                leftText.text = "A";

                pc.down = KeyCode.S;
                downText.text = "S";

                pc.right = KeyCode.D;
                rightText.text = "D";
                break;
            case 11:
                victoryText.gameObject.SetActive(true);
                break;
        }
    }

}
