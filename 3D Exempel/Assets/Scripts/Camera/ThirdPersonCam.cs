using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 15f;

    private CinemachineCamera cam;
    private PlayerControls controls;
    private CinemachineOrbitalFollow orbital;
    private Vector2 scrollDelta;

    private float targetZoom;
    private float currentZoom;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Enable();
        controls.CameraControls.MouseZoom.performed += HandleMouseScroll;

        cam = GetComponent<CinemachineCamera>();
        orbital = GetComponent<CinemachineOrbitalFollow>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        targetZoom = currentZoom = orbital.Radius;
    }

    private void HandleMouseScroll(InputAction.CallbackContext context)
    {
        scrollDelta = context.ReadValue<Vector2>();
        Debug.Log($"Mouse if scrolling. Value: {scrollDelta}");
    }

    void Update()
    {
        if (scrollDelta.y != 0)
        {
            if (orbital != null)
            {
                targetZoom = Mathf.Clamp (orbital.Radius - scrollDelta.y * zoomSpeed, minDistance, maxDistance);
                scrollDelta = Vector2.zero;
            }
        }
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);
        orbital.Radius = currentZoom;
    }
}
