using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;
using System;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] CinemachineCamera FPCam;
    [SerializeField] CinemachineCamera ISOCam;
    [SerializeField] CinemachineCamera TPCam;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchCamera(FPCam);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchCamera(ISOCam);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchCamera(TPCam);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ISOCam.Lens.OrthographicSize += 1; //Change values according to your requirements
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ISOCam.Lens.OrthographicSize -= 1;
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
