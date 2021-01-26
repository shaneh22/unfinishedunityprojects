using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VCam : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        _ = StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        while (true)
        {
            if(Player.instance == null)
            {
                yield return null;
            }
            else
            {
                cam.Follow = Player.instance.transform;
                break;
            }
        }
    }
}
