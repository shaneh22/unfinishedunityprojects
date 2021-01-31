using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VCam : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    private CinemachineFramingTransposer framingTransposer;

    public float shakeDuration;
    private float shakeElapsedTime;
    public float shakeAmplitude;

    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        virtualCameraNoise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        framingTransposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
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

    public void ScreenShake()
    {
        _ = StartCoroutine(Shake());
        shakeElapsedTime = 0;
    }

    private IEnumerator Shake()
    {
        while(shakeElapsedTime < shakeDuration)
        {
            virtualCameraNoise.m_AmplitudeGain = shakeAmplitude;
            shakeElapsedTime += Time.deltaTime;
            yield return null;
        }
        virtualCameraNoise.m_AmplitudeGain = 0;
    }

    public void LookDown()
    {
        framingTransposer.m_ScreenY = .1f;
    }
    public void ResetCameraPosition()
    {
        framingTransposer.m_ScreenY = .5f;
    }
}
