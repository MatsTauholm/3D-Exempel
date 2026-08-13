using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] CinemachineCamera FPCam;
    [SerializeField] CinemachineCamera TDCam;
    [SerializeField] CinemachineCamera TPCam;

    [SerializeField] GameObject PlayerFP;
    [SerializeField] GameObject PlayerTD;
    [SerializeField] GameObject PlayerTP;

    void Update()
    {
        if (Keyboard.current.digit1Key.isPressed)
        {
            SwitchCamera(FPCam, PlayerFP);
        }

        if (Keyboard.current.digit2Key.isPressed)
        {
            SwitchCamera(TDCam, PlayerTD);
        }

        if (Keyboard.current.digit3Key.isPressed)
        {
            SwitchCamera(TPCam, PlayerTP);
        }

        if (Mouse.current.scroll.y.ReadValue() > 0)
        {
            TDCam.Lens.OrthographicSize += 1; //Change values according to your requirements
        }

        if (Mouse.current.scroll.y.ReadValue() < 0)
        {
            TDCam.Lens.OrthographicSize -= 1;
        }
    }
    

    void SwitchCamera(CinemachineVirtualCameraBase activeCam, GameObject activePlayer)
    {
        // Set all priorities to a low value first
        FPCam.Priority = 0;
        TDCam.Priority = 0;
        TPCam.Priority = 0;

        // Boost the chosen one
        activeCam.Priority = 10;

        // Enable the selected player and disable the others
        PlayerFP.SetActive(activePlayer == PlayerFP);
        PlayerTD.SetActive(activePlayer == PlayerTD);
        PlayerTP.SetActive(activePlayer == PlayerTP);
    }
}
