using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelayTime;
    public float destroyGhostTime;

    public bool makeGhost;

    public GameObject ghost;

    private void Awake()
    {
        ghostDelayTime = ghostDelay;
    }

    private void Update()
    {
        if (makeGhost)
        {
            if (ghostDelayTime > 0)
            {
                ghostDelayTime -= Time.deltaTime;
            }
            else
            {
                GameObject currentGhost = Instantiate(ghost, transform.position, Quaternion.identity);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                currentGhost.transform.localScale = transform.localScale;
                ghostDelay = ghostDelayTime;
                Destroy(currentGhost, destroyGhostTime);
            }
        }
    }
}
