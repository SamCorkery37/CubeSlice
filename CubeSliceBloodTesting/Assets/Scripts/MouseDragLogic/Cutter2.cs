using System.Collections.Generic;
using UnityEngine;

public static class Cutter2
{
    private static bool isBusy;
    private static Mesh originalMesh;

    private static Material GetCrossSectionMaterial(GameObject originalGameObject)
    {
        Material objectMaterial = originalGameObject.GetComponent<Renderer>().material;
        return objectMaterial;
    }

    public static void Cut(GameObject originalGameObject, Vector3 contactPoint, Vector3 cutNormal)
    {
        if (isBusy) return;

        isBusy = true;

        try
        {
            // Debug: Track the original object position
            Debug.DrawRay(originalGameObject.transform.position, Vector3.up * 2f, Color.blue, 10f);  // Visualize original object's position

            Plane cutPlane = new Plane(originalGameObject.transform.InverseTransformDirection(-cutNormal), originalGameObject.transform.InverseTransformPoint(contactPoint));
            originalMesh = originalGameObject.GetComponent<MeshFilter>().mesh;

            if (originalMesh == null)
            {
                Debug.LogError("Need mesh to cut");
                return;
            }

            List<Vector3> addedVertices = new List<Vector3>();
            GeneratedMesh leftMesh = new GeneratedMesh();
            GeneratedMesh rightMesh = new GeneratedMesh();

            SeparateMeshes(leftMesh, rightMesh, cutPlane, addedVertices);
            FillCut(addedVertices, cutPlane, leftMesh, rightMesh);

            Mesh finishedLeftMesh = leftMesh.GetGeneratedMesh();
            Mesh finishedRightMesh = rightMesh.GetGeneratedMesh();

            originalGameObject.GetComponent<MeshFilter>().mesh = finishedLeftMesh;

            // Debug: Track the position of the left mesh
            Debug.DrawRay(originalGameObject.transform.position, Vector3.up * 2f, Color.green, 10f);  // Visualize new left mesh

            var originalCollider = originalGameObject.GetComponent<MeshCollider>();
            if (originalCollider != null)
            {
                Object.Destroy(originalCollider);
            }
            var newCollider = originalGameObject.AddComponent<MeshCollider>();
            newCollider.sharedMesh = finishedLeftMesh;
            newCollider.convex = true;

            ApplyMaterials(originalGameObject, finishedLeftMesh.subMeshCount, GetCrossSectionMaterial(originalGameObject));

            GameObject right = new GameObject("RightHalf");
            right.transform.position = originalGameObject.transform.position;
            right.transform.rotation = originalGameObject.transform.rotation;
            right.transform.localScale = originalGameObject.transform.localScale;

            right.AddComponent<MeshRenderer>();
            right.AddComponent<MeshFilter>().mesh = finishedRightMesh;
            var rightCollider = right.AddComponent<MeshCollider>();
            rightCollider.sharedMesh = finishedRightMesh;
            rightCollider.convex = true;

            // Debug: Track the position of the right half
            Debug.DrawRay(right.transform.position, Vector3.up * 2f, Color.red, 10f);  // Visualize right half
            Debug.Log($"RightHalf position: {right.transform.position}");

            ApplyMaterials(right, finishedRightMesh.subMeshCount, GetCrossSectionMaterial(originalGameObject));

            var rightRigidbody = right.AddComponent<Rigidbody>();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error during cutting: {ex.Message}");
        }
        finally
        {
            isBusy = false;
        }
    }

    private static void ApplyMaterials(GameObject gameObject, int subMeshCount, Material crossSectionMaterial)
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Material[] newMaterials = new Material[subMeshCount + 1];
            for (int i = 0; i < subMeshCount; i++)
            {
                newMaterials[i] = meshRenderer.material;
            }
            newMaterials[subMeshCount] = crossSectionMaterial;
            meshRenderer.materials = newMaterials;
        }
    }

    private static void SeparateMeshes(GeneratedMesh leftMesh, GeneratedMesh rightMesh, Plane plane, List<Vector3> addedVertices)
    {
        // Similar logic to Cutter.cs
    }

    private static void FillCut(List<Vector3> addedVertices, Plane plane, GeneratedMesh leftMesh, GeneratedMesh rightMesh)
    {
        // Similar logic to Cutter.cs
    }
}
