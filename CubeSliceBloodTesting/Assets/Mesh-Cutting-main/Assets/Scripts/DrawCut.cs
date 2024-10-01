using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class DrawCut : MonoBehaviour
{
    // public Transform boxVis;
    Vector3 pointA;
    Vector3 pointB;

    private LineRenderer cutRender;
    private bool animateCut;

    Camera cam;

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        cutRender = GetComponent<LineRenderer>();
        cutRender.startWidth = .05f;
        cutRender.endWidth = .05f;
    }

    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = -cam.transform.position.z;

        if (Input.GetMouseButtonDown(0))
        {
            pointA = cam.ScreenToWorldPoint(mouse);
        }

        if (Input.GetMouseButton(0))
        {
            animateCut = false;
            cutRender.SetPosition(0, pointA);
            cutRender.SetPosition(1, cam.ScreenToWorldPoint(mouse));
            cutRender.startColor = Color.gray;
            cutRender.endColor = Color.gray;
        }

        if (Input.GetMouseButtonUp(0))
        {
            pointB = cam.ScreenToWorldPoint(mouse);
            CreateSlicePlane();
            cutRender.positionCount = 2;
            cutRender.SetPosition(0, pointA);
            cutRender.SetPosition(1, pointB);
            animateCut = true;
        }

        if (animateCut)
        {
            cutRender.SetPosition(0, Vector3.Lerp(pointA, pointB, 1f));
        }
    }

    void CreateSlicePlane()
    {
        // The center point of the plane should be between pointA and pointB
        Vector3 pointInPlane = (pointA + pointB) / 2;

        // Calculate the normal of the cutting plane
        Vector3 cutPlaneNormal = Vector3.Cross((pointA - pointB), (pointA - cam.transform.position)).normalized;

        // Orientation of the cut plane
        Quaternion orientation = Quaternion.FromToRotation(Vector3.up, cutPlaneNormal);

        // Calculate the size of the slicing area with a width of 3 units
        Vector3 boxSize = new Vector3(3f, 0.01f, 3f); // Set width and depth to 3 units, and height to a small value for thin cuts

        // Check for objects in the slice area (OverlapBox allows control over slice size)
        var all = Physics.OverlapBox(pointInPlane, boxSize / 2, orientation);

        // Apply the cut to all objects within the slice area
        foreach (var hit in all)
        {
            MeshFilter filter = hit.gameObject.GetComponentInChildren<MeshFilter>();
            if (filter != null)
                Cutter.Cut(hit.gameObject, pointInPlane, cutPlaneNormal);
        }
    }

}
