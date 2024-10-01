using UnityEngine;

public class SwordSwing : MonoBehaviour
{
    public float raycastDistance;  // How far the SphereCast will go
    public LayerMask layerMask;             // LayerMask to filter objects
    public float sphereRadius = 0.5f;       // Radius of the SphereCast to make the ray "thicker"

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Left-click vertical swing
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Attempting vertical slice...");
            animator.SetTrigger("Swing");
            SliceObject(Vector3.up);  // Vertical slice
        }

        // Right-click horizontal swing
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Attempting horizontal slice...");
            animator.SetTrigger("Swing2");
            SliceObject(Vector3.right);  // Horizontal slice
        }
    }

    // Original slice logic (with LayerMask filtering and SphereCast)
    public void SliceObject(Vector3 sliceDirection)
    {
        // Draw the sphere cast to visualize its path
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.red, 1f);

        // Cast a "thicker" ray using SphereCast
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, raycastDistance, layerMask))
        {
            Debug.Log("SphereCast hit: " + hit.collider.name);

            // Check if the object hit has a slicable component
            if (hit.collider.CompareTag("Slicable"))
            {
                Debug.Log("Slicable object detected: " + hit.collider.name);

                // Slice the object based on the direction
                SlicerBlock slicer = hit.collider.GetComponent<SlicerBlock>();
                if (slicer != null)
                {
                    Debug.Log("Slicing the object...");
                    slicer.Slice(hit.point, sliceDirection);
                }
                else
                {
                    Debug.LogWarning("SlicerBlock component not found on the object.");
                }
            }
            else
            {
                Debug.Log("Object is not slicable.");
            }
        }
        else
        {
            Debug.Log("SphereCast did not hit anything.");
        }
    }
}
