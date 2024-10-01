using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class SwordSwing3 : MonoBehaviour
{
    public float raycastDistance = 12.5f;  // How far the SphereCast will go
    public LayerMask layerMask;            // LayerMask to filter objects
    public float sphereRadius = 2f;        // Radius of the SphereCast to make the ray "thicker"

    private Animator animator;
    [SerializeField] private Camera playerCamera;
    private RaycastHit hit;
    public Vector3 bloodOffset = Vector3.zero;
    [SerializeField] private Image bloodSplatterImage;

    public float bloodDisplayDuration = 2f;
    public float bloodFadeSpeed = 1f;
    public float initialAlpha = 0.5f;

    private Vector3 pointA;
    private Vector3 pointB;
    private bool dragging = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not assigned in the Inspector.");
        }

        if (bloodSplatterImage != null)
        {
            bloodSplatterImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            pointA = playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, playerCamera.nearClipPlane));
        }

        if (Input.GetMouseButtonUp(0) && dragging)
        {
            dragging = false;
            pointB = playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, playerCamera.nearClipPlane));
            AttemptDragSlice(pointA, pointB);
        }
    }

    public void AttemptDragSlice(Vector3 start, Vector3 end)
    {
        Vector3 sliceDirection = end - start;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.SphereCast(ray.origin, sphereRadius, ray.direction, out hit, raycastDistance, layerMask))
        {
            if (hit.collider.CompareTag("Slicable"))
            {
                Cutter2.Cut(hit.collider.gameObject, hit.point, sliceDirection.normalized);
            }
        }
    }

    void ShowBloodSplatter()
    {
        if (bloodSplatterImage != null)
        {
            bloodSplatterImage.gameObject.SetActive(true);
            bloodSplatterImage.color = new Color(bloodSplatterImage.color.r, bloodSplatterImage.color.g, bloodSplatterImage.color.b, initialAlpha);
            Invoke(nameof(HideBloodSplatter), bloodDisplayDuration);
        }
    }

    void HideBloodSplatter()
    {
        StartCoroutine(FadeOutBloodSplatter());
    }

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
