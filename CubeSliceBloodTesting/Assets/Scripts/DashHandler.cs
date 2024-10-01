using UnityEngine;

public class DashHandler : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 dashStartPosition;
    private Vector3 dashEndPosition;
    private float dashTime;
    private bool isDashing = false;

    // Dash parameters
    public float dashDistance = 10f;
    public float dashDuration = 0.2f;

    // Time scaling parameters with AnimationCurve
    public AnimationCurve timeScaleCurve;
    private float originalFixedDeltaTime;

    // Reference to the blood prefab settings
    public BFX_BloodSettings bloodSettings; // Assign this in the Inspector

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalFixedDeltaTime = Time.fixedDeltaTime; // Store original fixed delta time
    }

    // Call this method to start the dash
    public void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;

        // Calculate the dash end position based on the forward direction and dash distance
        dashStartPosition = transform.position;
        dashEndPosition = dashStartPosition + transform.forward * dashDistance;
    }

    // Handle the dash in Update
    public void HandleDash()
    {
        if (isDashing)
        {
            if (dashTime > 0)
            {
                // Interpolate the player's position between the start and end position over dashDuration
                float t = 1 - (dashTime / dashDuration); // Calculate interpolation factor
                Vector3 newPosition = Vector3.Lerp(dashStartPosition, dashEndPosition, t);

                // Move the player towards the dash end position
                controller.Move(newPosition - transform.position);

                // Adjust time scale using the animation curve
                float curveValue = timeScaleCurve.Evaluate(1 - (dashTime / dashDuration)); // Sample the curve based on the progress of the dash
                Time.timeScale = Mathf.Clamp(curveValue, 0.05f, 1f); // Ensure time scale doesn't go below 0.05 (very slow)
                Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale; // Adjust physics fixed time step

                // Adjust blood prefab animation speed
                if (bloodSettings != null)
                {
                    bloodSettings.AnimationSpeed = Time.timeScale * 0.5f; // Slow down the blood animation more during the dash
                }

                // Decrease dashTime using unscaled deltaTime to make the dash slow down correctly
                dashTime -= Time.unscaledDeltaTime;
            }
            else
            {
                EndDash();
            }
        }
    }

    // Call this method to end the dash
    public void EndDash()
    {
        isDashing = false;

        // Restore normal time scale after the dash
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime; // Reset fixed delta time

        // Reset blood animation speed to normal
        if (bloodSettings != null)
        {
            bloodSettings.AnimationSpeed = 1f; // Reset to normal speed
        }
    }

    public bool IsDashing()
    {
        return isDashing;
    }
}
