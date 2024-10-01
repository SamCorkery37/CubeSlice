using UnityEngine;

public class ShotGunButtHitBox : MonoBehaviour
{
    // The amount of force to apply when hitting an object
    public float knockbackForce = 500f;
    public float upwardForce = 300f;

    // LayerMask to specify the "Slicable" layer
    public LayerMask slicableLayer;

    // When the trigger collider detects a collision
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is in the "Slicable" layer
        if (slicableLayer == (slicableLayer | (1 << other.gameObject.layer)))
        {
            // Get the Rigidbody component of the object to apply force
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Calculate the direction to apply the force
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                Vector3 forceToApply = knockbackDirection * knockbackForce + Vector3.up * upwardForce;

                // Apply the force to the object's Rigidbody
                rb.AddForce(forceToApply, ForceMode.Impulse);

                // Optional: Add a debug log to confirm the hit
                Debug.Log("Hit Slicable object: " + other.gameObject.name);
            }
        }
    }
}
