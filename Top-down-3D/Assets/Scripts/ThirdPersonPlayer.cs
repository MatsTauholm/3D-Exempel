using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;

    public float rotationSmoothTime = 0.1f;

    private Rigidbody rb;

    private bool isGrounded;
    private bool jumpPressed;
    public float sphereRadius = 0.3f;
    private float turnSmoothVelocity;

    public LayerMask groundLayer;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        jumpPressed = true;
    }

    void FixedUpdate()
    {
        GroundCheck();
        Move();
    }

    void GroundCheck()
    {
        Vector3 checkPosition = transform.position + Vector3.down * 0.1f;
        isGrounded = Physics.CheckSphere(checkPosition, sphereRadius, groundLayer);

        Debug.DrawLine(transform.position, checkPosition, isGrounded ? Color.green : Color.red);
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

        // Preserve existing vertical velocity
        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetVel = moveDir * moveSpeed;
        targetVel.y = currentVel.y;

        rb.linearVelocity = targetVel;
    }

    void Jump()
    {
        if (jumpPressed && isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce));
            jumpPressed = false;
        }
    }
}

