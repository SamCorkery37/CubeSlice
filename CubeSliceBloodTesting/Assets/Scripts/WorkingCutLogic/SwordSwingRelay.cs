// using UnityEngine;

// public class SwordSwingRelay : MonoBehaviour
// {
//     // Reference to the SwordSwingHoldRelease script on the sword
//     public SwordSwingHoldRelease swordScript;

//     // This method will be called by animation events to enable the hitbox
//     public void EnableHitbox()
//     {
//         if (swordScript != null)
//         {
//             Debug.Log("Relaying event to enable hitbox.");
//             swordScript.EnableHitbox();  // Call the EnableHitbox method on the sword
//         }
//         else
//         {
//             Debug.LogWarning("SwordSwingHoldRelease script is not assigned!");
//         }
//     }

//     // This method will be called by animation events to disable the hitbox
//     public void DisableHitbox()
//     {
//         if (swordScript != null)
//         {
//             Debug.Log("Relaying event to disable hitbox.");
//             swordScript.DisableHitbox();  // Call the DisableHitbox method on the sword
//         }
//         else
//         {
//             Debug.LogWarning("SwordSwingHoldRelease script is not assigned!");
//         }
//     }
// }
