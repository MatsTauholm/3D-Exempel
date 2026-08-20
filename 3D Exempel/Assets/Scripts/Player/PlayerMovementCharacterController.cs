using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovementCharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSmoothTime = 0.1f;
    public float gravity = -9.81f;       // Gravity force
    public float jumpSpeed = 2f;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 inputDir;
    private Vector3 velocity; 
    private float turnSmoothVelocity;
    private bool shouldJump;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        shouldJump = true; 
    }

    private void Update()
    {
        Move();
        JumpAndGravity();
        RotateTowardMouse();
    }

    private void Move()
    {
        // Convert 2D input into a 3D direction
        inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        // If there's no input, don't move
        if (inputDir.sqrMagnitude < 0.01f)
        {
            return;
        }
            
        // Get camera’s forward & right (flattened to ground plane)
        Transform cam = Camera.main.transform;
        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;

        // Calculate movement direction relative to camera
        Vector3 moveDir = camRight * inputDir.x + camForward * inputDir.z;

        // Move the character
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    void JumpAndGravity()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // small negative to keep grounded
        }

        // Jump
        if (shouldJump && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
            shouldJump = false;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move vertically
        controller.Move(velocity * Time.deltaTime);
    }

    void RotateTowardMouse() // Smoothly rotate to face move direction if moving
    {
        if (inputDir.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                                                      ref turnSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }
    }
}