using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPosition;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !GameManager.instance.spaceshipDead)
        {
            Shoot();
        }
    }
    private void Shoot()
    {
        _ = Instantiate(bullet, bulletPosition);
    }
}
