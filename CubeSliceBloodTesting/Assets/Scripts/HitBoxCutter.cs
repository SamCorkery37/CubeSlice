using UnityEngine;

public class HitboxCutter : MonoBehaviour
{
    public LayerMask layerMask;  // Layers to interact with
    private Vector3 lastPosition;
    private Collider hitboxCollider;
    private Rigidbody rb;
    public float deathForce = 2f;

    private int cooldownFrames = 15;  // Set your cooldown in frames
    private int currentFrame = 0;     // Track the current frame count

    void Start()
    {
        lastPosition = transform.position;  // Track initial position
        hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider == null)
        {
            Debug.LogError("Collider not found on hitbox!");
        }
    }

    void Update()
    {
        // Update the last known position for movement direction calculation
        lastPosition = transform.position;

        // Increment the cooldown frame counter in Update
        if (currentFrame < cooldownFrames)
        {
            currentFrame++;
        }
    }

    public void EnableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;
            Debug.Log("Hitbox enabled.");
        }
    }

    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
            Debug.Log("Hitbox disabled.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slicable") && currentFrame >= cooldownFrames)
        {
            // Calculate movement direction to define the cut plane normal
            Vector3 movementDirection = (transform.position - lastPosition).normalized;

            // Define the vertical direction
            Vector3 verticalDirection = Vector3.up;

            // Calculate horizontal direction based on the sword or player's orientation (local right vector)
            Vector3 horizontalDirection = transform.right;  // Or player.transform.right for player-based

            // Add more weight to the horizontal direction to force a horizontal cut
            Vector3 weightedHorizontalDirection = horizontalDirection * 3f;  // Increase horizontal influence
            Vector3 weightedMovementDirection = movementDirection * 1f;  // Keep movement influence
            Vector3 weightedVerticalDirection = verticalDirection * 0.2f;  // Reduce vertical influence

            Vector3 combinedDirection = (weightedHorizontalDirection + weightedMovementDirection + weightedVerticalDirection).normalized;

            // Define the cut position as the closest point of the object
            Vector3 contactPoint = other.ClosestPoint(transform.position);

            // Trigger the cut using the combined direction
            Cutter.Cut(other.gameObject, contactPoint, combinedDirection);

            Debug.Log("Cut made with hitbox at: " + contactPoint + " with combined normal: " + combinedDirection);
            ApplyDeathForce();

            // Reset the cooldown frame counter
            currentFrame = 0;
        }
    }

    void ApplyDeathForce()
    {
        // Make the Rigidbody no longer kinematic, so physics can affect it
        rb.isKinematic = false;

        // Apply force with more vertical influence and less horizontal influence for a realistic "stumble" effect
        Vector3 randomForceDirection = new Vector3(Random.Range(-0.5f, 2f), 0.8f, Random.Range(-0.5f, 2f)).normalized;

        // Reduce deathForce magnitude for less extreme propulsion
        rb.AddForce(randomForceDirection * (deathForce * 0.5f), ForceMode.Impulse);

        // Apply random torque to simulate stumbling or falling over
        Vector3 randomTorque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.AddTorque(randomTorque * (deathForce * 0.3f), ForceMode.Impulse);

        Debug.Log("Death force applied: " + randomForceDirection * deathForce);
    }

}
