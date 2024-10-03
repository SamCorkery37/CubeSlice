using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float acceleration = 10f; // Speed adjustment for acceleration
    public float deceleration = 10f; // Speed adjustment for deceleration
    private float currentSpeed;

    private Vector3 velocity;
    private bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // Cinemachine references for screen shake
    public CinemachineImpulseSource impulseSource;

    // Camera head bob settings
    public Transform cameraTransform;
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;
    private float defaultCameraYPos;
    private float bobTimer = 0f;

    // Animator reference
    public Animator animator;

    // Dash Handler reference
    private DashHandler dashHandler;

    // Speed modification
    private float targetSpeed;
    public float reducedSpeed = 3f; // Speed while holding Shift

    public Transform playerStartPosition;

    void Start()
    {
        // Initial setup
        transform.position = playerStartPosition.position;
        transform.rotation = playerStartPosition.rotation;

        targetSpeed = walkSpeed; // Set initial speed to walking speed
        currentSpeed = targetSpeed;

        // Setup camera head bob
        defaultCameraYPos = cameraTransform.localPosition.y;
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keeps the player grounded
        }

        // Get input for movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Handle running (Shift key)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            targetSpeed = runSpeed; // Run while Shift is held
        }
        else
        {
            targetSpeed = walkSpeed; // Walk when Shift is released
        }

        // Smooth acceleration and deceleration
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // Move the player
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Walking animation
        if (animator != null)
        {
            bool isMoving = move.magnitude > 0 && isGrounded; // Only "walking" when grounded
            animator.SetBool("IsWalking", isMoving); // Assumes you have an "IsWalking" parameter in your Animator
        }

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            impulseSource.GenerateImpulse(); // Trigger screen shake on jump
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Apply head bobbing effect only when the player is grounded and moving
        if (move.magnitude > 0 && isGrounded)
        {
            BobHead();
        }
        else
        {
            ResetHeadBob(); // Ensure the head bob resets if the player is not grounded or not moving
        }
    }


    // Head bobbing effect when walking or running
    void BobHead()
    {
        bobTimer += Time.deltaTime * bobSpeed;
        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            defaultCameraYPos + Mathf.Sin(bobTimer) * bobAmount,
            cameraTransform.localPosition.z
        );
    }

    // Reset head bob when not moving
    void ResetHeadBob()
    {
        bobTimer = 0f;
        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            defaultCameraYPos,
            cameraTransform.localPosition.z
        );
    }
}
