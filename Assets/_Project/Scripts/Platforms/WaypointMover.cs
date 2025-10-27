using UnityEngine;
using System.Collections;

public class WaypointMover : MonoBehaviour
{
    [Header("Waypoints Settings")]
    public Transform[] waypoints;         // Assign at least 2
    public float moveSpeed = 3f;          // Speed of movement
    public float reachThreshold = 0.05f;  // How close before it's considered "arrived"

    private int currentIndex = 0;
    private bool isMoving = false;
    private Coroutine moveRoutine;

    // Move forward through waypoints
    public void GoToNextWaypoint()
    {
        if (waypoints.Length < 2) return;

        int nextIndex = currentIndex + 1;
        if (nextIndex >= waypoints.Length)
            nextIndex = waypoints.Length - 1; // clamp to last

        StartMove(nextIndex);
    }

    // Move backward through waypoints
    public void GoToLastWaypoint()
    {
        if (waypoints.Length < 2) return;

        int prevIndex = currentIndex - 1;
        if (prevIndex < 0)
            prevIndex = 0; // clamp to first

        StartMove(prevIndex);
    }

    private void StartMove(int targetIndex)
    {
        if (isMoving || targetIndex == currentIndex) return;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToWaypoint(targetIndex));
    }

    private IEnumerator MoveToWaypoint(int targetIndex)
    {
        isMoving = true;

        Vector3 start = transform.position;
        Vector3 target = waypoints[targetIndex].position;
        float distance = Vector3.Distance(start, target);

        while (Vector3.Distance(transform.position, target) > reachThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        currentIndex = targetIndex;
        isMoving = false;
    }
}
