using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    private Vector3 velocity;
    private bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // Animator reference
    public Animator animator;

    // Dash Handler reference
    private DashHandler dashHandler;

    // Speed modification
    public float normalSpeed = 6f; // Default player speed
    public float reducedSpeed = 3f; // Speed while holding Shift

    public Transform playerStartPosition;

    void Start()
    {

        transform.position = playerStartPosition.position;
        transform.rotation = playerStartPosition.rotation;
        dashHandler = GetComponent<DashHandler>(); // Get the reference to the DashHandler script
        speed = normalSpeed; // Set initial speed to normal
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

        // Handle dash input
        if (Input.GetKey(KeyCode.LeftShift) && !dashHandler.IsDashing())
        {
            speed = reducedSpeed; // Reduce speed while Shift is held
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && !dashHandler.IsDashing())
        {
            dashHandler.StartDash(); // Start the dash when Shift is released
            speed = normalSpeed; // Reset speed after dash
        }

        // Handle dashing
        dashHandler.HandleDash();

        // Move the player with normal movement if not dashing
        if (!dashHandler.IsDashing())
        {
            controller.Move(move * speed * Time.deltaTime);
        }

        // Walking animation
        if (animator != null)
        {
            bool isMoving = move.magnitude > 0;
            animator.SetBool("IsWalking", isMoving); // Assumes you have an "IsWalking" parameter in your Animator
        }

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
