using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSmoothTime = 0.1f;
    public float gravity = -9.81f;       // Gravity force
    public float jumpSpeed = 2f;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private float turnSmoothVelocity;
    private bool jumpPressed;
    private Vector3 velocity; // vertical velocity

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
        jumpPressed = true; 
    }

    private void Update()
    {
        Move();
        JumpAndGravity();
    }

    private void Move()
    {
        // Convert 2D input into a 3D direction
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        if (inputDir.sqrMagnitude < 0.01f) return;

        // Get camera’s forward & right (flattened to ground plane)
        Transform cam = Camera.main.transform;
        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;

        Vector3 moveDir = camRight * inputDir.x + camForward * inputDir.z;

        // Smoothly rotate player to face movement direction
        float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

        // Move the character
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    void JumpAndGravity()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            // Reset downward velocity when grounded
            velocity.y = -2f; // small negative to keep grounded
        }

        // Jump
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
            jumpPressed = false;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move vertically
        controller.Move(velocity * Time.deltaTime);
    }

}