using UnityEngine;
using UnityEngine.UI; // Needed to work with UI
using System.Collections;

public class SwordSwing2 : MonoBehaviour
{
    public float raycastDistance = 12.5f;  // How far the SphereCast will go
    public LayerMask layerMask;            // LayerMask to filter objects
    public float sphereRadius = 2f;        // Radius of the SphereCast to make the ray "thicker"

    private Animator animator;

    [SerializeField] private Camera playerCamera;  // Reference to the First Person Camera
    private RaycastHit hit;  // Store the hit information for Gizmos visualization
    public Vector3 bloodOffset = Vector3.zero;  // Offset for blood prefab position adjustment

    // Reference to the blood splatter UI Image
    [SerializeField] private Image bloodSplatterImage;

    [Header("Blood Splatter Settings")]
    public float bloodDisplayDuration = 2f; // How long the blood stays on the screen (adjustable in Inspector)
    public float bloodFadeSpeed = 1f;       // Speed of blood fading out (adjustable in Inspector)
    public float initialAlpha = 0.5f;       // The starting alpha for the blood splatter (adjustable in Inspector)

    void Start()
    {
        animator = GetComponent<Animator>();

        // Optionally, check if the camera is assigned
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not assigned in the Inspector.");
        }

        // Ensure the blood splatter image is initially hidden
        if (bloodSplatterImage != null)
        {
            bloodSplatterImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Check if Fire1 is pressed down
        if (Input.GetButton("Fire1"))
        {
            Debug.Log("Holding for vertical slice...");
        }

        // Check if Fire1 is released
        if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Attempting vertical slice...");
            animator.SetTrigger("Swing");
            AttemptSlice(Vector3.up);  // Vertical slice direction
        }

        // Check if Fire2 is pressed down
        if (Input.GetButton("Fire2"))
        {
            Debug.Log("Holding for horizontal slice...");
        }

        // Check if Fire2 is released
        if (Input.GetButtonUp("Fire2"))
        {
            Debug.Log("Attempting horizontal slice...");
            animator.SetTrigger("Swing2");
            AttemptSlice(Vector3.right);  // Horizontal slice direction
        }
    }

    // Attempts to slice objects with the sword, using the camera's direction (center of the screen)
    public void AttemptSlice(Vector3 sliceDirection)
    {
        // Cast a ray from the center of the screen (crosshair) using the camera
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // Perform a SphereCast using the camera's ray direction
        if (Physics.SphereCast(ray.origin, sphereRadius, ray.direction, out hit, raycastDistance, layerMask))
        {
            Debug.Log("SphereCast hit: " + hit.collider.name + " on Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

            // Check if the object is on the layer you want to slice (using the LayerMask)
            if (((1 << hit.collider.gameObject.layer) & layerMask) != 0)
            {
                // Ensure the object hit has a mesh and is slicable (based on tag or other criteria)
                if (hit.collider.CompareTag("Slicable"))
                {
                    Debug.Log("Slicable object detected: " + hit.collider.name);

                    // Use Cutter to slice the object
                    Cutter.Cut(hit.collider.gameObject, hit.point, sliceDirection);

                    // Activate the blood effect on the screen
                    ShowBloodSplatter();

                    // Activate the blood prefab (if it exists) and set its position
                    GameObject bloodPrefab = hit.collider.transform.Find("BloodPrefab")?.gameObject;
                    if (bloodPrefab != null)
                    {
                        // Set the blood prefab's position to the hit point plus the offset
                        bloodPrefab.transform.position = hit.point + bloodOffset;

                        // Optionally, adjust the rotation of the blood prefab to align with the hit surface
                        bloodPrefab.transform.rotation = Quaternion.LookRotation(hit.normal);

                        bloodPrefab.SetActive(true);
                        Debug.Log("Blood prefab activated on: " + hit.collider.name);
                    }
                    else
                    {
                        Debug.Log("No blood prefab found on: " + hit.collider.name);
                    }
                }
                else
                {
                    Debug.Log("Object is not slicable, but is in allowed layer.");
                }
            }
            else
            {
                Debug.Log("Object is on an excluded layer (e.g., ground), cannot slice.");
            }
        }
        else
        {
            Debug.Log("SphereCast did not hit anything.");
        }
    }

    // Method to display blood splatter on screen
    void ShowBloodSplatter()
    {
        if (bloodSplatterImage != null)
        {
            bloodSplatterImage.gameObject.SetActive(true);
            // Set initial alpha to make the blood less opaque at the start
            bloodSplatterImage.color = new Color(bloodSplatterImage.color.r, bloodSplatterImage.color.g, bloodSplatterImage.color.b, initialAlpha);
            Invoke(nameof(HideBloodSplatter), bloodDisplayDuration);  // Hide after a delay
        }
    }

    // Method to hide blood splatter with a fade effect
    void HideBloodSplatter()
    {
        StartCoroutine(FadeOutBloodSplatter());
    }

    // Coroutine to fade out the blood splatter image
    IEnumerator FadeOutBloodSplatter()
    {
        Color imageColor = bloodSplatterImage.color;
        while (imageColor.a > 0f)
        {
            imageColor.a -= Time.deltaTime * bloodFadeSpeed;
            bloodSplatterImage.color = imageColor;
            yield return null;
        }
        bloodSplatterImage.gameObject.SetActive(false);
    }
}
