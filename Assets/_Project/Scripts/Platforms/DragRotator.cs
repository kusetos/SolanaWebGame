using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DragRotator : MonoBehaviour
{
    private Camera cam;
    private Rigidbody2D rb;
    private bool isDragging;

    private float startAngle;   // object angle when drag started
    private float startMouseAngle; // mouse angle when drag started

    void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMouseDown()
    {
        isDragging = true;

        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - (Vector2)transform.position;

        startMouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        startAngle = rb.rotation;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (!isDragging) return;

        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - (Vector2)transform.position;

        float currentMouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Calculate how far the mouse rotated since drag start
        float angleDelta = Mathf.DeltaAngle(startMouseAngle, currentMouseAngle);

        // Apply that rotation to the object’s original angle
        rb.MoveRotation(startAngle + angleDelta);
    }
}
