using UnityEngine;

public class ParentAnimatorHandler : MonoBehaviour
{
    // References to both hitbox scripts on the child objects
    private HitboxCutter hitbox1;
    private HitboxCutter hitbox2;

    void Start()
    {
        // Find both hitbox scripts in the child objects
        hitbox1 = GetComponentsInChildren<HitboxCutter>()[0];  // First hitbox
        hitbox2 = GetComponentsInChildren<HitboxCutter>()[1];  // Second hitbox

        if (hitbox1 == null || hitbox2 == null)
        {
            Debug.LogError("HitboxCutter scripts not found in child objects!");
        }
    }

    // Method to enable both hitboxes
    public void EnableHitboxes()
    {
        if (hitbox1 != null && hitbox2 != null)
        {
            hitbox1.EnableHitbox();
            hitbox2.EnableHitbox();
        }
    }

    // Method to disable both hitboxes
    public void DisableHitboxes()
    {
        if (hitbox1 != null && hitbox2 != null)
        {
            hitbox1.DisableHitbox();
            hitbox2.DisableHitbox();
        }
    }
}
