using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public void Collision()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        Invoke(nameof(Reset), 3f);
    }
    private void Reset()
    {
        gameObject.SetActive(true);
    }
}
