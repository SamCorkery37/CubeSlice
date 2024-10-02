using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    private Animator animator;
    private bool isPlayerInRange = false;  // Track if the player is in range

    void Start()
    {
        animator = GetComponent<Animator>();  // Get the Animator component on the door
        if (animator == null)
        {
            Debug.LogError("Animator component missing on the door!");
        }
    }

    void Update()
    {
        // Check if the player is in range and presses the "E" key
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E.");
            // Trigger the OpenDoor animation
            animator.SetTrigger("OpenDoor");
        }
    }

    // When the player enters the trigger zone
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;  // Player is in range
            Debug.Log("Player entered door range.");
        }
    }

    // When the player exits the trigger zone
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;  // Player is out of range
            Debug.Log("Player exited door range.");
        }
    }
}
