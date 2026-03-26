using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementThirdPerson : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float rotationSmoothTime = 0.1f;
    [SerializeField] float sphereRadius = 0.3f;
    [SerializeField] LayerMask groundLayer;

    private bool isGrounded;
    private bool shouldJump;
    private float turnSmoothVelocity;
    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        shouldJump = true;
    }

    private void Update()
    {
        GroundCheck();
        Move();
    }

    void FixedUpdate()
    { 
        Jump();
    }

    void GroundCheck()
    {
        // Check if the player is grounded by checking a sphere below the player
        Vector3 checkPosition = capsule.bounds.center - Vector3.up * capsule.bounds.extents.y;
        isGrounded = Physics.CheckSphere(checkPosition, sphereRadius, groundLayer);
    }

    void Move()
    {
        // Camera-relative input direction
        Transform cam = Camera.main.transform;
        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;

        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 moveDir = camRight * inputDir.x + camForward * inputDir.z;

        // Smoothly rotate to face move direction if moving
        if (moveDir.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                                                      ref turnSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        // Set the player's velocity while preserving the current vertical velocity (y-axis)    
        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
    }

    void Jump()
    {
        if (shouldJump && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce));
            shouldJump = false;
        }
    }
}