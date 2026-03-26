using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class PlayerMovementFirstPerson : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float sphereRadius = 0.3f;
    [SerializeField] LayerMask groundLayer;

    private bool isGrounded;
    private float radius;
    private Rigidbody rb;
    private Vector2 moveInput;
    private CapsuleCollider capsule;
    private bool shouldJump;

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

    void Update()
    {
        GroundCheck();
        
    }

    void FixedUpdate()
    {
        Move();
        Jump();
    }

    void GroundCheck()
    {
        // Check if the player is grounded by checking a sphere below the player
        Vector3 checkPosition = capsule.bounds.center - Vector3.up * capsule.bounds.extents.y;
        isGrounded = Physics.CheckSphere(checkPosition, sphereRadius, groundLayer);

        //Debug if needed
        //Debug.DrawLine(transform.position, checkPosition, isGrounded ? Color.green : Color.red);
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
        Vector3 targetVelocity = (camRight * moveInput.x + camForward * moveInput.y) * moveSpeed;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(targetVelocity.x, -2f, targetVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
    }

    void Jump()
    {
        Debug.Log("Attempting to jump. Grounded: " + isGrounded);
        if (isGrounded && shouldJump)
        {
            rb.AddForce(new Vector2(0, jumpForce));
            shouldJump = false;
        }
    }
}