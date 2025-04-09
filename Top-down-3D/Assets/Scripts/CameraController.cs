using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using System;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] CinemachineVirtualCamera FPCam;
    [SerializeField] CinemachineVirtualCamera ISOCam;
    [SerializeField] CinemachineFreeLook TPCam;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchCamera(FPCam);
            mainCam.orthographic = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchCamera(ISOCam);
            mainCam.orthographic = true;
            ISOCam.m_Lens.OrthographicSize = 25f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchCamera(TPCam);
            mainCam.orthographic = false;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ISOCam.m_Lens.OrthographicSize += 1; //Change values according to your requirements
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ISOCam.m_Lens.OrthographicSize -= 1;
        }
    }
    

    void SwitchCamera(CinemachineVirtualCameraBase activeCam)
    {
        // Set all priorities to a low value first
        FPCam.Priority = 0;
        ISOCam.Priority = 0;
        TPCam.Priority = 0;

        // Boost the chosen one
        activeCam.Priority = 10;
    }
}
