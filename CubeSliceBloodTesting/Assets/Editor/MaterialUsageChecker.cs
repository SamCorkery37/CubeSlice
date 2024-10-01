using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MaterialUsageChecker : MonoBehaviour
{
    [MenuItem("Tools/Check Material Usage")]
    public static void CheckMaterials()
    {
        // Find all renderers in the scene
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        HashSet<Material> uniqueMaterials = new HashSet<Material>();

        // Loop through each renderer and collect unique materials
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.sharedMaterials) // Use sharedMaterials to avoid creating instances
            {
                if (mat != null)
                {
                    uniqueMaterials.Add(mat);
                }
            }
        }

        // Output the result
        Debug.Log("Unique materials in scene: " + uniqueMaterials.Count);
    }
}
