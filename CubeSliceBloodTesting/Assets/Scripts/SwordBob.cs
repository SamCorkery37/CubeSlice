using UnityEngine;

public class SwordBob : MonoBehaviour
{
    public float bobbingSpeed = 5f;   // How fast the sword bobs
    public float bobbingAmount = 0.1f; // How much the sword bobs
    private float timer = 0f;

    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial local position of the sword
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float waveslice = 0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Check if the player is moving
        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            // Player is not moving, reset the timer but maintain current position
            timer = 0f;
        }
        else
        {
            // Player is moving, apply bobbing effect
            waveslice = Mathf.Sin(timer);
            timer += bobbingSpeed * Time.deltaTime;

            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }
        }

        // Calculate new sword position, relative to the initial position
        Vector3 swordPosition = initialPosition;

        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            swordPosition.y += translateChange;  // Adjust the Y position dynamically
        }

        // Apply the bobbing movement to the sword
        transform.localPosition = swordPosition;
    }
}
