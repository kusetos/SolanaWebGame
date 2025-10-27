using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;    
    public bool faceRight = true;         // Whether the object initially faces right

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

            if (!faceRight)
                targetAngle -= 90f;

            float newAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);

            rb.MoveRotation(newAngle);
        }
    }
}

