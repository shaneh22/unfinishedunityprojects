using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int powerUpID;
    public GameObject screen;
    public BoxCollider2D bc;
    // Start is called before the first frame update
    private void Awake()
    {
        screen.SetActive(false);
    }
    private void OnEnable()
    {
        screen.SetActive(false);
        _ = StartCoroutine(AlreadyCollected());
    }

    private IEnumerator AlreadyCollected()
    {
        while (true)
        {
            if(Player.instance != null)
            {
                switch (powerUpID)
                {
                    case 1:
                        if (Player.instance.vengeanceEnabled)
                        {
                            Destroy(gameObject);
                        }
                        break;
                    case 2:
                        if (Player.instance.dashEnabled)
                        {
                            Destroy(gameObject);
                        }
                        break;
                    case 3:
                        if (Player.instance.doubleJumpEnabled)
                        {
                            Destroy(gameObject);
                        }
                        break;
                    case 4:
                        if (Player.instance.wallJumpEnabled)
                        {
                            Destroy(gameObject);
                        }
                        break;
                }
                break;
            }
            yield return null;
        }
    }

    public void Collison()
    {
        switch (powerUpID)
        {
            case 1:
                Player.instance.vengeanceEnabled = true;
                break;
            case 2:
                Player.instance.dashEnabled = true;
                break;
            case 3:
                Player.instance.doubleJumpEnabled = true;
                break;
            case 4:
                Player.instance.wallJumpEnabled = true;
                break;
        }
        screen.SetActive(true);
        Invoke(nameof(CloseScreen), 1.9f);
    }

    private void CloseScreen()
    {
        screen.SetActive(false);
        Destroy(gameObject);
    }
}
