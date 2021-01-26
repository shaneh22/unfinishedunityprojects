using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] private GameObject player;

    void Start()
    {
        if(Player.instance == null)
        {
            _ = Instantiate(player, transform.position, Quaternion.identity);
        }
    }
}
