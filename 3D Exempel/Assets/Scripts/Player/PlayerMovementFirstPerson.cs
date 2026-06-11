using System;
using System.Data;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerMovementFirstPerson : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float gravityScale = 3f;
    private bool isSprinting;
    private float moveForce;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float airMultiplier = 2.5f;
    [SerializeField] private float airDrag = 2f;
    private bool shouldJump;

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float sphereRadius = 0.3f;
    private bool isGrounded;

    [Header("Slope Handling Settings")]
    [SerializeField] private float maxSlopeAngle = 45f; // Maximum slope angle the player can walk on
    [SerializeField] private PhysicsMaterial zeroFriction;
    [SerializeField] private PhysicsMaterial normalFriction;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private CapsuleCollider capsule;
    
    #region State machine variables (not in use)
    //public MovementState state;
    //public enum MovementState
    //{
    //    walking,
    //    sprinting,
    //    crouching,
    //    air
    //}
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponentInChildren<CapsuleCollider>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Lock the cursor to the center of the screen and hide it for a first-person experience
        rb.freezeRotation = true;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        if(isGrounded)
        {
            shouldJump = true;
        }
    }

    void Update()
    {
        GroundCheck();
        ApplyDrag();
        //StateHandler(); //WiP
        SpeedControl();
        SetMaterial();
    }

    private void SetMaterial() // Set the physics material based on whether the player is on a slope or not to prevent sliding and getting stuck on walls
    {
        if (OnSlope())
        {
            capsule.material = normalFriction;
        }
        else
        {
            capsule.material = zeroFriction;
        }
    }

    //private void StateHandler() // Handle the player's movement state and set the move force accordingly
    //{
    //    // Mode - Crouching
    //    if (isCrouching)
    //    {
    //        state = MovementState.crouching;
    //        moveForce = crouchSpeed;
    //    }

    //    // Mode - Sprinting
    //    else if (isGrounded && isSprinting)
    //    {
    //        state = MovementState.sprinting;
    //        moveForce = sprintSpeed;
    //    }

    //    // Mode - Walking
    //    else if (isGrounded)
    //    {
    //        state = MovementState.walking;
    //        moveForce = moveSpeed;
    //    }

    //    // Mode - Air
    //    else
    //    {
    //        state = MovementState.air;
    //    }
    //}

    void GroundCheck() // Check if the player is grounded by checking a sphere below the player
    {
        Vector3 checkPosition = capsule.bounds.center + Vector3.down * capsule.bounds.extents.y;
        isGrounded = Physics.CheckSphere(checkPosition, sphereRadius, groundLayer);
    }

    private void ApplyDrag() // Change drag in the air to allow for better air control
    {
        if(isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else 
        {
            rb.linearDamping = airDrag; 
        }
    }

    private void SpeedControl() //Regulate the player's speed in different situations 
    { 
        if (OnSlope() && !exitingSlope) // Limiting speed on slope
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }    
        }
        else // Limiting speed on ground or in air
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Get the horizontal velocity (ignoring vertical velocity)

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private bool OnSlope() //Return true if the player is on a slope
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, capsule.bounds.extents.y + 0.5f)) // Cast a ray downwards to check for the slope angle
        {
            float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal); // Calculate the angle between the up vector and the slope normal
            return slopeAngle != 0 && slopeAngle < maxSlopeAngle; // Return true if the slope angle is greater than 0 and less than the maximum slope angle
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection() // Get the movement direction adjusted for the slope to allow for smooth movement on slopes
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized; // Project the movement direction onto the slope plane defined by the slope normal and normalize it to maintain consistent movement speed
    }

    void FixedUpdate()
    {
        Move();
        SetGravity();
        Jump();
    }

    private void SetGravity() // Apply custom gravity to allow for better control on slopes and in the air
    {
        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
        rb.useGravity = !OnSlope(); // Disable gravity when on slope to prevent sliding down
    }

    void Move()
    {
        // Get the camera's forward and right vectors
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Ignore vertical direction (y-axis) to prevent moving up/down when looking up/down
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize(); //  Normalize to ensure consistent movement speed in all directions
        camRight.Normalize();

        // Move relative to camera
        moveDirection = camRight * moveInput.x + camForward * moveInput.y;

        if(isGrounded)
        {
            if (OnSlope() && !exitingSlope) //On a slope
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 15f, ForceMode.Force);
                if (rb.linearVelocity.y > 0) // Prevent sliding up slopes
                {
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                }
            }
            else // On flat ground
            {
                rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
            }
        }
        else // In the air
        {
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    void Jump()
    {
        if (isGrounded && shouldJump)
        {
            exitingSlope = true;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(new Vector2(0, jumpForce), ForceMode.Impulse);
            Invoke(nameof(ResetJump), jumpCooldown);
            shouldJump = false;
        }
    }

    private void ResetJump()
    {
        shouldJump = false;
        exitingSlope = false;
    }
}