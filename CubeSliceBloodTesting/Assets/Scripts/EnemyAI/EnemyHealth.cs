using UnityEngine;
using UnityEngine.AI;  // For NavMeshAgent

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Rigidbody rb;
    public float deathForce = 10f;
    private NavMeshAgent navMeshAgent;  // Reference to NavMeshAgent

    void Start()
    {
        // Set the current health to max health at the start
        currentHealth = maxHealth;

        // Get the NavMeshAgent component attached to this enemy
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent not found on the enemy!");
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();  // Add Rigidbody if none exists
        }

        rb.isKinematic = true;  // Make kinematic while alive
    }

    // Call this function to apply damage to the enemy
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;  // Reduce current health by the damage amount
        Debug.Log("Enemy took damage, current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Disable the NavMeshAgent component to stop movement
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        Debug.Log("Enemy has died!");

        // Trigger the cutting logic on death
        PerformCut();
        ApplyDeathForce();

        // You can add additional code here for death behavior, such as:
        // - Playing a death animation
        // - Removing the enemy from the scene
        // - Dropping loot, etc.
    }

    void PerformCut()
    {
        // You can use the same cutting logic here as before
        Vector3 movementDirection = transform.forward.normalized;  // Assuming forward is the death direction

        // Define the vertical direction
        Vector3 verticalDirection = Vector3.up;

        // Calculate horizontal direction based on the enemy's orientation
        Vector3 horizontalDirection = transform.right;  // Local right vector of the enemy

        // Add more weight to the horizontal direction to force a horizontal cut
        Vector3 weightedHorizontalDirection = horizontalDirection * 3f;  // Increase horizontal influence
        Vector3 weightedMovementDirection = movementDirection * 1f;  // Keep movement influence
        Vector3 weightedVerticalDirection = verticalDirection * 0.2f;  // Reduce vertical influence

        Vector3 combinedDirection = (weightedHorizontalDirection + weightedMovementDirection + weightedVerticalDirection).normalized;

        // Define the cut position as the enemy's position
        Vector3 contactPoint = transform.position;

        // Trigger the cut
        Cutter.Cut(gameObject, contactPoint, combinedDirection);

        Debug.Log("Cut made at death with combined normal: " + combinedDirection);
    }

    void ApplyDeathForce()
    {
        // Make the Rigidbody no longer kinematic, so physics can affect it
        rb.isKinematic = false;

        // Apply force in a random direction for dramatic effect
        Vector3 randomForceDirection = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;

        // Apply force with the adjustable deathForce value
        rb.AddForce(randomForceDirection * deathForce, ForceMode.Impulse);

        Debug.Log("Death force applied: " + randomForceDirection * deathForce);
    }
}
