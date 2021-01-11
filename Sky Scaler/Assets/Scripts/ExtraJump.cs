using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    public void Collision()
    {
        gameObject.SetActive(false);
        Invoke(nameof(Reset), 3f);
    }
    private void Reset()
    {
        gameObject.SetActive(true);
    }
}
