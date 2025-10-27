using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Player or object to follow

    [Header("Settings")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Axis Locking")]
    public bool lockX = false;
    public bool lockY = false;

    private float startX;
    private float startY;

    private void Start()
    {
        // Store the starting camera position
        startX = transform.position.x;
        startY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Desired target position
        Vector3 desiredPosition = target.position + offset;

        // Apply locking
        if (lockX) desiredPosition.x = startX;
        if (lockY) desiredPosition.y = startY;

        // Smooth follow
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
