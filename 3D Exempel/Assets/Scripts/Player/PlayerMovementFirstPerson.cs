using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovementFirstPerson : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveForce = 5f;
    [SerializeField] float airForce = 2.5f;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float groundDrag = 5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 5f;    
    [SerializeField] float rotationSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    
    [Header("Ground Check Settings")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float sphereRadius = 0.3f;
    private bool isGrounded;

    [Header("Slope Handling Settings")]
    [SerializeField] float maxSlopeAngle = 45f; // Maximum slope angle the player can walk on
    private RaycastHit slopeHit;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private CapsuleCollider capsule;
    private bool shouldJump;
    

    void Start()
    {
        Physics.gravity = new Vector3(0, gravity, 0);
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
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
    }

    void GroundCheck()
    {
        // Check if the player is grounded by checking a sphere below the player
        Vector3 checkPosition = capsule.bounds.center - Vector3.up * capsule.bounds.extents.y;
        isGrounded = Physics.CheckSphere(checkPosition, sphereRadius, groundLayer);

        //Debug if needed
        //Debug.DrawLine(transform.position, checkPosition, isGrounded ? Color.green : Color.red);
    }

    private void ApplyDrag()
    {
        if(isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0f; // No drag in the air for more responsive jumping/falling
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, capsule.bounds.extents.y + 0.5f, groundLayer))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            return slopeAngle > 0 && slopeAngle <= maxSlopeAngle;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    void FixedUpdate()
    {
        Move();
        Jump();
    }

    #region Movement
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
        moveDirection = (camRight * moveInput.x + camForward * moveInput.y);

        //If on ground and/or on a slope
        if(isGrounded)
        {
            if (OnSlope())
            {
                rb.AddForce(GetSlopeMoveDirection() * moveForce, ForceMode.Force);
                if (rb.linearVelocity.y > 0) // Prevent sliding up slopes
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else
            {
                rb.AddForce(moveDirection.normalized * moveForce, ForceMode.Force);
            }
        }
        else // In air - allow some control but less than on ground
            rb.AddForce(moveDirection.normalized * airForce, ForceMode.Force);

        rb.useGravity = !OnSlope(); // Disable gravity when on slope to prevent sliding down

        //Speed cap to prevent excessive velocity buildup
        rb.linearVelocity = new Vector3(
            Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed),
            rb.linearVelocity.y,
            Mathf.Clamp(rb.linearVelocity.z, -maxSpeed, maxSpeed)
        );

    }
    #endregion

    #region Rotation Toward Mouse
    void RotateTowardMouse()
    {
        // Smoothly rotate to face move direction if moving
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                                                      ref turnSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }
    }
    #endregion

    #region Jumping
    void Jump()
    {
        if (isGrounded && shouldJump)
        {
            rb.AddForce(new Vector2(0, jumpForce));
            shouldJump = false;
        }
    }
    #endregion
}