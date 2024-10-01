using UnityEngine;
using UnityEngine.UI;

public class DrawCut2 : MonoBehaviour
{
    public RectTransform canvasRectTransform;  // UI canvas for the line
    public RectTransform cutLineUI;            // UI line to draw the cut
    public Transform sword;
    public float swordLength = 2f;

    private Vector2 pointA;
    private Vector2 pointB;
    private Camera cam;

    public float raycastDistance = 12.5f;  // How far the SphereCast will go
    public LayerMask layerMask;            // LayerMask to filter objects
    public float sphereRadius = 2f;        // Radius of the SphereCast to make the ray "thicker"

    private RaycastHit hit;
    private bool isDrawing = false;

    void Start()
    {
        cam = Camera.main;
        cutLineUI.gameObject.SetActive(false);  // Ensure the line is hidden initially
    }

    void Update()
    {
        // Start drawing the line when the player clicks
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            pointA = Input.mousePosition;
            cutLineUI.gameObject.SetActive(true);  // Show the line when starting the cut
            cutLineUI.anchoredPosition = pointA;   // Set the start position
        }

        // Update the line's end position as the player drags the mouse
        if (Input.GetMouseButton(0) && isDrawing)
        {
            pointB = Input.mousePosition;
            DrawUILine(pointA, pointB);  // Update the UI line in 2D space
        }

        // End the line drawing and perform the cut
        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false;
            cutLineUI.gameObject.SetActive(false);  // Hide the line after the cut
            CreateSlicePlane();  // Perform the cut based on the drawn line
        }
    }

    void DrawUILine(Vector2 start, Vector2 end)
    {
        // Calculate the direction and distance between the points
        Vector2 direction = end - start;
        float distance = direction.magnitude;

        // Set the size and rotation of the UI line to match the drawn line
        cutLineUI.sizeDelta = new Vector2(distance, cutLineUI.sizeDelta.y);
        cutLineUI.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    void CreateSlicePlane()
    {
        // Convert the 2D points into a slice plane in world space
        Vector3 pointInPlane = cam.ScreenToWorldPoint(new Vector3((pointA.x + pointB.x) / 2, (pointA.y + pointB.y) / 2, cam.nearClipPlane));
        Vector3 cutPlaneNormal = Vector3.Cross((cam.ScreenToWorldPoint(new Vector3(pointA.x, pointA.y, cam.nearClipPlane)) - cam.ScreenToWorldPoint(new Vector3(pointB.x, pointB.y, cam.nearClipPlane))), cam.transform.forward).normalized;

        // Perform a SphereCast along the drawn line to detect sliceable objects
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red, 2f);

        // You can also visualize a line showing the sphere radius for a clearer understanding
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * sphereRadius, Color.blue, 2f);


        if (Physics.SphereCast(ray.origin, sphereRadius, ray.direction, out hit, raycastDistance, layerMask))
        {
            Debug.Log("SphereCast hit: " + hit.collider.name + " on Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

            if (hit.collider.CompareTag("Slicable"))
            {
                // Call Cutter to perform the slice
                Cutter.Cut(hit.collider.gameObject, hit.point, cutPlaneNormal);
            }
        }
        else
        {
            Debug.Log("SphereCast did not hit anything.");
        }
    }
}