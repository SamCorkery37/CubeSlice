using UnityEngine;

public class ThirdPersonMouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;  // Assign the capsule to this

    private float xRotation = 0f;

    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (look up/down) with clamping (on the camera)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 30f);  // Limit vertical rotation to avoid flipping over
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player body left and right (horizontal rotation on the capsule)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
