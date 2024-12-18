using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TestCinemachine : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Material mat;
    public Color color;

    private void Start()
    {
        color = mat.GetColor("_Color");
        virtualCamera.DestroyCinemachineComponent<CinemachineTransposer>();
    }
}
