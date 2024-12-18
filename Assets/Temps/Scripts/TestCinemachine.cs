using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TestCinemachine : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        virtualCamera.DestroyCinemachineComponent<CinemachineTransposer>();
    }
}
