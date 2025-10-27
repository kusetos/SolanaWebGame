using UnityEngine;

public class PlatformPathDrag : MonoBehaviour
{
    [Header("Waypoints Path")]
    public Transform[] waypoints; // The path points
    public float moveSpeed = 10f; // Smooth move speed

    private Camera mainCamera;
    private bool isDragging = false;
    private float tAlongPath = 0f; // Value from 0..1 along path

    void Start()
    {
        mainCamera = Camera.main;
        if (waypoints.Length < 2)
            Debug.LogWarning("You need at least 2 waypoints for path movement!");
    }

    void Update()
    {
        if (isDragging && waypoints.Length >= 2)
        {
            Vector3 mousePos = GetMouseWorldPosition();

            // Find closest point ON the path to mouse position
            Vector3 closestPoint = GetClosestPointOnPath(mousePos, out float t);
            tAlongPath = Mathf.Clamp01(t);

            // Smoothly move platform to that position
            transform.position = Vector3.Lerp(transform.position, closestPoint, Time.deltaTime * moveSpeed);
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(mouseScreen);
    }

    Vector3 GetClosestPointOnPath(Vector3 point, out float tResult)
    {
        // We’ll find the closest segment and the closest point on it
        Vector3 closestPoint = Vector3.zero;
        float minDist = float.MaxValue;
        float totalLength = GetPathLength();
        float accumulatedLength = 0f;
        float bestT = 0f;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Vector3 a = waypoints[i].position;
            Vector3 b = waypoints[i + 1].position;
            Vector3 segmentClosest = ClosestPointOnSegment(a, b, point);
            float dist = Vector3.Distance(point, segmentClosest);
            float segmentLength = Vector3.Distance(a, b);

            if (dist < minDist)
            {
                minDist = dist;
                closestPoint = segmentClosest;

                // Calculate normalized t along full path
                float tSegment = Vector3.Distance(a, segmentClosest) / segmentLength;
                bestT = (accumulatedLength + tSegment * segmentLength) / totalLength;
            }

            accumulatedLength += segmentLength;
        }

        tResult = bestT;
        return closestPoint;
    }
    // --- Add this part ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform.parent == transform)
                collision.transform.SetParent(null);
        }
    }
    Vector3 ClosestPointOnSegment(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(point - a, ab) / Vector3.Dot(ab, ab);
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }

    float GetPathLength()
    {
        float length = 0f;
        for (int i = 0; i < waypoints.Length - 1; i++)
            length += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        return length;
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] && waypoints[i + 1])
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.DrawSphere(waypoints[i].position, 0.1f);
            }
        }
        Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.1f);
    }
}
