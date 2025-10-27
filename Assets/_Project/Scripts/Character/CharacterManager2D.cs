using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterManager2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float airControlMultiplier = 0.5f;
    

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Animation Parameters")]
    [SerializeField] private string moveParam = "walk";
    //[SerializeField] private string idleParam = "idle";
    // [SerializeField] private string velocityYParam = "VelocityY";
    // [SerializeField] private string landTrigger = "Land";
    
    [Header("Events")]
    public UnityEvent OnJump;
    public UnityEvent OnLand;
    public UnityEvent<float> OnMove;
    //public UnityEvent OnDash;
    
    // Components
    private Rigidbody2D rb;
    public Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // State
    private float moveInput;
    private float currentSpeed;
    private bool isGrounded;
    private bool wasGrounded;
    private float coyoteTimeCounter;
    //private float jumpBufferCounter;
    
    // Properties
    //public bool IsGrounded => isGrounded;
    //public float MoveInput => moveInput;
    //public Vector2 Velocity => rb.linearVelocity;
    public bool IsFacingRight { get; private set; } = true;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        
        // Configure Rigidbody2D for better platformer feel
        rb.gravityScale = 3f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    
    private void Update()
    {
        CheckGrounded();
        HandleCoyoteTime();
        //HandleJumpBuffer();
        UpdateAnimations();
        
        // Check for landing
        if (isGrounded && !wasGrounded)
        {
            Debug.Log("LANDED");
            OnLanded();
        }
        
        wasGrounded = isGrounded;
    }
    
    private void FixedUpdate()
    {
        ApplyMovement();
    }
    
    
    public void SetMoveInput(float input)
    {
        moveInput = Mathf.Clamp(input, -1f, 1f);
    }
    
    
    public void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    public void SetFacingDirection(bool facingRight)
    {
        if (IsFacingRight != facingRight)
        {
            Flip();
        }
    }
    
    
    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapBox(
            groundCheck.position, 
            groundCheckSize, 
            0f, 
            groundLayer
        );
        
    }
    
    private void HandleCoyoteTime()
    {
        if (coyoteTimeCounter > 0)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }
    
   
    private void ApplyMovement()
    {
        float targetSpeed = moveInput * moveSpeed;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        
        if (!isGrounded)
        {
            accelRate *= airControlMultiplier;
        }
        
        currentSpeed = Mathf.MoveTowards(
            currentSpeed, 
            targetSpeed, 
            accelRate * Time.fixedDeltaTime
        );
        
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
        
        // Handle sprite flipping
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            SetFacingDirection(moveInput > 0);
            OnMove?.Invoke(moveInput);
        }
    }
    
    // private void ApplyBetterJump()
    // {
    //     // Apply extra gravity when falling
    //     if (rb.linearVelocity.y < 0)
    //     {
    //         rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
    //     }
    //     // Apply extra gravity when releasing jump button
    //     else if (rb.linearVelocity.y > 0 && jumpBufferCounter <= 0)
    //     {
    //         rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    //     }
    // }
    
    private void OnLanded()
    {
        OnLand?.Invoke();
    }
    
    private void UpdateAnimations()
    {
        if (animator == null) return;

        bool isWalking = moveInput != 0;

        animator.SetBool(moveParam, isWalking);
        // if (isWalking)
        // {
        //     SoundManager.Instance.Play("walk");
        // }
        //animator.SetBool(idleParam, !isWalking);
        //animator.SetFloat(velocityYParam, rb.linearVelocity.y);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}