using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public int speed;
    private void FixedUpdate()
    {
        gameObject.transform.Rotate(new Vector3(0, 0, -speed));
    }
}
