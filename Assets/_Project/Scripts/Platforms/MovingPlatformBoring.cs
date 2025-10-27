using UnityEngine;

public class MovingPlatformBoring : MonoBehaviour
{
    [Header("Waypoints Settings")]
    public Transform[] waypoints; // Points the platform moves between
    public float moveSpeed = 3f;  // Speed of movement
    public bool loop = true;      // Loop back to start
    public bool pingPong = false; // Move back and forth

    private int currentIndex = 0;
    private bool movingForward = true;

    void Update()
    {
        if (waypoints.Length == 0) return;

        // Move platform toward current waypoint
        Transform target = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            NextWaypoint();
        }
    }

    void NextWaypoint()
    {
        if (pingPong)
        {
            if (movingForward)
            {
                currentIndex++;
                if (currentIndex >= waypoints.Length)
                {
                    currentIndex = waypoints.Length - 2;
                    movingForward = false;
                }
            }
            else
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 1;
                    movingForward = true;
                }
            }
        }
        else
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                if (loop)
                    currentIndex = 0;
                else
                    enabled = false; // Stop moving
            }
        }
    }

    // Optional: Draw waypoints in editor
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);

            if (i < waypoints.Length - 1 && waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
