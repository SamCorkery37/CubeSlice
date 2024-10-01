using EzySlice;
using UnityEngine;

public class SlicerBlock : MonoBehaviour
{
    public Material crossSectionMaterial;  // Material for the cross-section after slicing
    private float sliceForce = 10f;        // Adjustable force applied to the sliced parts
    public float randomFactor = 0.3f;      // Adds some randomness to the force

    private Rigidbody rb;  // Reference to the block's rigidbody

    void Start()
    {
        // Ensure only the original block is set to kinematic before slicing
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;  // Only original block is set to kinematic
        }
    }

    public void Slice(Vector3 hitPoint, Vector3 sliceDirection)
    {
        Debug.Log("Attempting to slice object at: " + hitPoint);

        // Explicitly use EzySlice's Plane
        EzySlice.Plane slicingPlane = new EzySlice.Plane(sliceDirection, hitPoint);
        Debug.Log("Slicing plane created. Normal: " + slicingPlane.normal);

        // Perform the slice using EzySlice's Slice method
        SlicedHull hull = gameObject.Slice(slicingPlane, crossSectionMaterial);

        if (hull != null)
        {
            Debug.Log("Slicing successful!");

            // Create both halves from the sliced object
            GameObject upperHull = hull.CreateUpperHull(gameObject, crossSectionMaterial);
            GameObject lowerHull = hull.CreateLowerHull(gameObject, crossSectionMaterial);

            // Set up the upper and lower hulls with the correct physics and slicing ability
            if (upperHull != null)
            {
                SetupSlicedHull(upperHull, hitPoint, slicingPlane.normal, true);
            }

            if (lowerHull != null)
            {
                SetupSlicedHull(lowerHull, hitPoint, slicingPlane.normal, false);
            }

            // Destroy the original object after slicing
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Slicing failed.");
        }
    }

    private void SetupSlicedHull(GameObject hull, Vector3 hitPoint, Vector3 sliceNormal, bool isUpperHull)
    {
        // Match original position and rotation
        hull.transform.position = transform.position;
        hull.transform.rotation = transform.rotation;

        // Assign the same layer as the original object to the hulls
        hull.layer = gameObject.layer;

        // Add physics and slicing functionality to the hull
        AddPhysicsToSlice(hull, hitPoint, sliceNormal, isUpperHull);
        AddSlicingComponent(hull);  // Add the SlicerBlock component to make it sliceable
        EnsureMeshRenderer(hull);   // Ensure MeshRenderer exists
    }

    private void AddSlicingComponent(GameObject hull)
    {
        // Add the SlicerBlock component and transfer properties
        SlicerBlock slicer = hull.AddComponent<SlicerBlock>();
        slicer.crossSectionMaterial = this.crossSectionMaterial;  // Transfer the material
        slicer.sliceForce = this.sliceForce;  // Optional: Transfer other relevant properties
        slicer.randomFactor = this.randomFactor;
        Debug.Log("SlicerBlock component added to hull.");
    }

    private void AddPhysicsToSlice(GameObject slice, Vector3 hitPoint, Vector3 sliceNormal, bool isUpperHull)
    {
        // Add a Rigidbody component to the slice and ensure it is non-kinematic
        Rigidbody sliceRb = slice.AddComponent<Rigidbody>();

        // Ensure Rigidbody is set to non-kinematic
        sliceRb.isKinematic = false;
        Debug.Log("Rigidbody added to slice. IsKinematic: " + sliceRb.isKinematic);

        // Add a convex MeshCollider for proper collision detection
        MeshCollider collider = slice.AddComponent<MeshCollider>();
        collider.convex = true;

        // Determine force direction based on whether it's the upper or lower hull
        Vector3 forceDirection = (isUpperHull ? sliceNormal : -sliceNormal) + Random.insideUnitSphere * randomFactor;

        // Apply force to blow the hulls apart visually
        sliceRb.AddForce(forceDirection.normalized * sliceForce * 10f, ForceMode.Impulse);  // Stronger force for visual feedback
    }

    private void EnsureMeshRenderer(GameObject obj)
    {
        if (!obj.GetComponent<MeshRenderer>())
        {
            obj.AddComponent<MeshRenderer>();
            Debug.LogWarning("MeshRenderer added to sliced object.");
        }
    }
}
