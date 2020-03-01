using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text controlsText;
    public TMP_Text titleText;

    private void Start()
    {
        StartCoroutine(Directions());
        controlsText = GameObject.Find("Controls").GetComponent<TMP_Text>();
        titleText = GameObject.Find("TitleText").GetComponent<TMP_Text>();
        controlsText.gameObject.SetActive(false);
    }
    private IEnumerator Directions()
    {
        while (true)
        {
            if (Input.anyKeyDown)
            {
                controlsText.gameObject.SetActive(true);
                titleText.gameObject.SetActive(false);
                yield return new WaitForSeconds(.2f);
                while (true)
                {
                    if (Input.anyKeyDown)
                    {
                        SceneManager.LoadScene(1);
                    }
                    yield return null;
                }
            }
            yield return null;
        }
    }
}
