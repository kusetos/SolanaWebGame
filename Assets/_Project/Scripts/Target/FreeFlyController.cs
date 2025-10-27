using UnityEngine;

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FreeFlyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public bool canMove;
    public float moveSpeed = 5f;     
    public float acceleration = 10f;      
    public float deceleration = 10f;      

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 targetVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;            // No gravity for flying
        rb.freezeRotation = true;        // Don't rotate on collision
    }

    void Update()
    {
        if (!canMove) return;   
        // Read input (WASD or Arrow keys)
        input = new Vector2(
            Input.GetAxisRaw("Horizontal"),  // A/D or Left/Right
            Input.GetAxisRaw("Vertical")     // W/S or Up/Down
        ).normalized;
    }

    void FixedUpdate()
    {
        // Calculate target velocity based on input
        targetVelocity = input * moveSpeed;

        // Smooth acceleration / deceleration
        Vector2 currentVelocity = rb.linearVelocity;
        Vector2 newVelocity = Vector2.zero;

        if (input.magnitude > 0)
        {
            // Accelerate toward target velocity
            newVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Slow down gradually when no input
            newVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = newVelocity;
    }
}
