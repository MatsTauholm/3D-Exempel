using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementTopDown : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;

    private bool isGrounded;
    public float sphereRadius = 0.3f;
    private Rigidbody rb;
    public LayerMask groundLayer;

    private RaycastHit Hit;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        Look();
    }

    void Look()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 lookAtPoint = hit.point;
            lookAtPoint.y = gameObject.transform.position.y; // Keep the same y position as the player
            gameObject.transform.LookAt(lookAtPoint);
            
        }
    }

    void FixedUpdate()
    {
        GroundCheck();
        Move();
        Jump();
    }

    void GroundCheck()
    {
        Vector3 checkPosition = transform.position + Vector3.down * 0.1f;
        isGrounded = Physics.CheckSphere(checkPosition, sphereRadius, groundLayer);

        Debug.DrawLine(transform.position, checkPosition, isGrounded ? Color.green : Color.red);
    }

    private void Move()
    {
        // Get the camera's forward and right vectors
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Ignore vertical direction (y-axis)
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Move relative to camera
        Vector3 targetVelocity = (camRight * moveInput.x + camForward * moveInput.y) * moveSpeed;

        // Keep current vertical velocity
        Vector3 velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;

        rb.linearVelocity = velocity;
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce));
        }
    }
}
