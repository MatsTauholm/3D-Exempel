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

     [SerializeField] GameObject PlayerFP;
     [SerializeField] GameObject PlayerISO;
     [SerializeField] GameObject PlayerTP;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchCamera(FPCam, PlayerFP);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchCamera(ISOCam, PlayerISO);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchCamera(TPCam, PlayerTP);
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
    

    void SwitchCamera(CinemachineVirtualCameraBase activeCam, GameObject activePlayer)
    {
        // Set all priorities to a low value first
        FPCam.Priority = 0;
        ISOCam.Priority = 0;
        TPCam.Priority = 0;

        // Boost the chosen one
        activeCam.Priority = 10;

        // Enable the selected player and disable the others
        PlayerFP.SetActive(activePlayer == PlayerFP);
        PlayerISO.SetActive(activePlayer == PlayerISO);
        PlayerTP.SetActive(activePlayer == PlayerTP);
    }
}
