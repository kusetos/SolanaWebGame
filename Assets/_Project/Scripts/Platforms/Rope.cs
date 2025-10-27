using UnityEngine;
using System.Collections.Generic;

public class Rope : MonoBehaviour
{
    [Header("Rope Setup")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private int segmentCount = 20;
    [SerializeField] private float ropeLength = 5f;
    [SerializeField] private bool autoCalculateLength = true;
    
    [Header("Visual Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float ropeWidth = 0.1f;
    [SerializeField] private Material ropeMaterial;
    [SerializeField] private Color ropeColor = Color.brown;
    
    [Header("Physics Settings")]
    [SerializeField] private float segmentMass = 0.1f;
    [SerializeField] private float springForce = 500f;
    [SerializeField] private float damping = 5f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private bool useColliders = true;
    [SerializeField] private float colliderRadius = 0.05f;
    
    [Header("Connection Settings")]
    [SerializeField] private bool pinStartPoint = true;
    [SerializeField] private bool pinEndPoint = false;
    [SerializeField] private float maxStretchDistance = 0.5f;
    [SerializeField] private bool canBreak = false;
    [SerializeField] private float breakForce = 100f;
    
    [Header("Interaction Settings")]
    [SerializeField] private bool canGrab = true;
    [SerializeField] private LayerMask interactionLayer = -1;
    [SerializeField] private float grabRadius = 0.3f;
    [SerializeField] private bool affectsObjects = true;
    
    [Header("Wind & External Forces")]
    [SerializeField] private bool enableWind = false;
    [SerializeField] private Vector2 windForce = Vector2.zero;
    [SerializeField] private float windVariation = 0.5f;
    
    // Rope segments
    private List<RopeSegment> segments = new List<RopeSegment>();
    private float segmentLength;
    private RopeSegment grabbedSegment;
    private Vector3 grabOffset;
    private bool isBroken = false;
    
    // Properties
    public bool IsBroken => isBroken;
    public int SegmentCount => segments.Count;
    public float CurrentLength => CalculateCurrentLength();
    public bool IsGrabbing => grabbedSegment != null;
    
    private class RopeSegment
    {
        public GameObject gameObject;
        public Rigidbody2D rigidbody;
        public CircleCollider2D collider;
        public Vector2 position;
        public Vector2 oldPosition;
        public bool isPinned;
        public int index;
    }
    
    private void Start()
    {
        InitializeRope();
    }
    
    private void Update()
    {
        if (isBroken) return;
        
        HandleGrabInput();
        UpdateLineRenderer();
    }
    
    private void FixedUpdate()
    {
        if (isBroken) return;
        
        ApplyPhysics();
        ApplyConstraints();
        CheckForBreak();
        
        if (enableWind)
        {
            ApplyWind();
        }
    }
    
    // ============= INITIALIZATION =============
    
    private void InitializeRope()
    {
        if (startPoint == null)
        {
            Debug.LogError("Start point not assigned!");
            return;
        }
        
        // Calculate rope length
        if (autoCalculateLength && endPoint != null)
        {
            ropeLength = Vector2.Distance(startPoint.position, endPoint.position);
        }
        
        segmentLength = ropeLength / segmentCount;
        
        // Create line renderer if not assigned
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        SetupLineRenderer();
        CreateRopeSegments();
    }
    
    private void SetupLineRenderer()
    {
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;
        lineRenderer.positionCount = segmentCount + 1;
        lineRenderer.material = ropeMaterial != null ? ropeMaterial : new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = ropeColor;
        lineRenderer.endColor = ropeColor;
        lineRenderer.sortingOrder = 10;
        lineRenderer.useWorldSpace = true;
    }
    
    private void CreateRopeSegments()
    {
        segments.Clear();
        
        Vector2 startPos = startPoint.position;
        Vector2 direction = endPoint != null ? 
            ((Vector2)endPoint.position - startPos).normalized : 
            Vector2.down;
        
        for (int i = 0; i <= segmentCount; i++)
        {
            Vector2 position = startPos + direction * (segmentLength * i);
            
            RopeSegment segment = new RopeSegment
            {
                position = position,
                oldPosition = position,
                isPinned = (i == 0 && pinStartPoint) || (i == segmentCount && pinEndPoint),
                index = i
            };
            
            // Create GameObject for physics
            segment.gameObject = new GameObject($"RopeSegment_{i}");
            segment.gameObject.transform.parent = transform;
            segment.gameObject.transform.position = position;
            segment.gameObject.layer = gameObject.layer;
            
            // Add Rigidbody2D
            segment.rigidbody = segment.gameObject.AddComponent<Rigidbody2D>();
            segment.rigidbody.mass = segmentMass;
            segment.rigidbody.gravityScale = gravityScale;
            segment.rigidbody.linearDamping = damping;
            segment.rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            if (segment.isPinned)
            {
                segment.rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }
            
            // Add Collider
            if (useColliders)
            {
                segment.collider = segment.gameObject.AddComponent<CircleCollider2D>();
                segment.collider.radius = colliderRadius;
            }
            
            segments.Add(segment);
        }
        
        // Connect segments with joints for better physics
        ConnectSegmentsWithJoints();
    }
    
    private void ConnectSegmentsWithJoints()
    {
        for (int i = 0; i < segments.Count - 1; i++)
        {
            DistanceJoint2D joint = segments[i].gameObject.AddComponent<DistanceJoint2D>();
            joint.connectedBody = segments[i + 1].rigidbody;
            joint.autoConfigureDistance = false;
            joint.distance = segmentLength;
            joint.maxDistanceOnly = false;
            joint.enableCollision = false;
        }
    }
    
    // ============= PHYSICS =============
    
    private void ApplyPhysics()
    {
        // Update pinned segments
        if (pinStartPoint && startPoint != null)
        {
            segments[0].position = startPoint.position;
            segments[0].rigidbody.MovePosition(startPoint.position);
        }
        
        if (pinEndPoint && endPoint != null && segments.Count > 0)
        {
            int lastIndex = segments.Count - 1;
            segments[lastIndex].position = endPoint.position;
            segments[lastIndex].rigidbody.MovePosition(endPoint.position);
        }
        
        // Update positions from rigidbodies
        foreach (var segment in segments)
        {
            if (!segment.isPinned)
            {
                segment.position = segment.rigidbody.position;
            }
        }
    }
    
    private void ApplyConstraints()
    {
        // Apply distance constraints between segments
        for (int i = 0; i < segments.Count - 1; i++)
        {
            RopeSegment segmentA = segments[i];
            RopeSegment segmentB = segments[i + 1];
            
            Vector2 delta = segmentB.position - segmentA.position;
            float distance = delta.magnitude;
            float difference = (distance - segmentLength) / distance;
            
            Vector2 offset = delta * difference * 0.5f;
            
            if (!segmentA.isPinned)
            {
                segmentA.position += offset;
                segmentA.rigidbody.MovePosition(segmentA.position);
            }
            
            if (!segmentB.isPinned)
            {
                segmentB.position -= offset;
                segmentB.rigidbody.MovePosition(segmentB.position);
            }
        }
        
        // Limit maximum stretch
        for (int i = 0; i < segments.Count - 1; i++)
        {
            float currentDist = Vector2.Distance(segments[i].position, segments[i + 1].position);
            if (currentDist > segmentLength + maxStretchDistance)
            {
                Vector2 direction = (segments[i + 1].position - segments[i].position).normalized;
                segments[i + 1].position = segments[i].position + direction * (segmentLength + maxStretchDistance);
                segments[i + 1].rigidbody.MovePosition(segments[i + 1].position);
            }
        }
    }
    
    private void ApplyWind()
    {
        Vector2 wind = windForce;
        
        foreach (var segment in segments)
        {
            if (!segment.isPinned)
            {
                // Add some randomness to wind
                Vector2 randomWind = wind + Random.insideUnitCircle * windVariation;
                segment.rigidbody.AddForce(randomWind);
            }
        }
    }
    
    private void CheckForBreak()
    {
        if (!canBreak) return;
        
        for (int i = 0; i < segments.Count - 1; i++)
        {
            float distance = Vector2.Distance(segments[i].position, segments[i + 1].position);
            float strain = distance - segmentLength;
            
            if (strain > breakForce * 0.01f) // Convert to distance threshold
            {
                BreakRope(i);
                return;
            }
        }
    }
    
    private void BreakRope(int segmentIndex)
    {
        isBroken = true;
        
        // Remove joints from broken segment
        if (segmentIndex < segments.Count - 1)
        {
            DistanceJoint2D joint = segments[segmentIndex].gameObject.GetComponent<DistanceJoint2D>();
            if (joint != null)
            {
                Destroy(joint);
            }
        }
        
        Debug.Log($"Rope broke at segment {segmentIndex}!");
    }
    
    // ============= INTERACTION =============
    
    private void HandleGrabInput()
    {
        if (!canGrab) return;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        if (Input.GetMouseButtonDown(0))
        {
            TryGrabSegment(mousePos);
        }
        
        if (Input.GetMouseButton(0) && grabbedSegment != null)
        {
            DragSegment(mousePos);
        }
        
        if (Input.GetMouseButtonUp(0) && grabbedSegment != null)
        {
            ReleaseSegment();
        }
    }
    
    private void TryGrabSegment(Vector3 mousePos)
    {
        foreach (var segment in segments)
        {
            if (!segment.isPinned && Vector2.Distance(segment.position, mousePos) < grabRadius)
            {
                grabbedSegment = segment;
                grabOffset = segment.position - (Vector2)mousePos;
                segment.rigidbody.bodyType = RigidbodyType2D.Kinematic;
                break;
            }
        }
    }
    
    private void DragSegment(Vector3 mousePos)
    {
        Vector2 targetPos = (Vector2)mousePos + (Vector2)grabOffset;
        grabbedSegment.position = targetPos;
        grabbedSegment.rigidbody.MovePosition(targetPos);
    }
    
    private void ReleaseSegment()
    {
        if (grabbedSegment != null)
        {
            grabbedSegment.rigidbody.bodyType = RigidbodyType2D.Dynamic;
            grabbedSegment = null;
        }
    }
    
    // ============= VISUAL =============
    
    private void UpdateLineRenderer()
    {
        if (lineRenderer == null) return;
        
        for (int i = 0; i < segments.Count; i++)
        {
            lineRenderer.SetPosition(i, segments[i].position);
        }
    }
    
    // ============= PUBLIC METHODS =============
    
    public void AttachObjectToEnd(GameObject obj)
    {
        if (segments.Count == 0) return;
        
        Rigidbody2D objRb = obj.GetComponent<Rigidbody2D>();
        if (objRb != null)
        {
            RopeSegment lastSegment = segments[segments.Count - 1];
            
            DistanceJoint2D joint = lastSegment.gameObject.AddComponent<DistanceJoint2D>();
            joint.connectedBody = objRb;
            joint.autoConfigureDistance = true;
            joint.maxDistanceOnly = true;
            
            // Unpin end if it was pinned
            if (pinEndPoint)
            {
                lastSegment.isPinned = false;
                lastSegment.rigidbody.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
    
    public void SetWindForce(Vector2 force)
    {
        windForce = force;
    }
    
    public void CutRope(int segmentIndex)
    {
        if (segmentIndex >= 0 && segmentIndex < segments.Count - 1)
        {
            BreakRope(segmentIndex);
        }
    }
    
    public Vector2 GetSegmentPosition(int index)
    {
        if (index >= 0 && index < segments.Count)
        {
            return segments[index].position;
        }
        return Vector2.zero;
    }
    
    public void ApplyForceToSegment(int index, Vector2 force)
    {
        if (index >= 0 && index < segments.Count && !segments[index].isPinned)
        {
            segments[index].rigidbody.AddForce(force);
        }
    }
    
    private float CalculateCurrentLength()
    {
        float length = 0f;
        for (int i = 0; i < segments.Count - 1; i++)
        {
            length += Vector2.Distance(segments[i].position, segments[i + 1].position);
        }
        return length;
    }
    
    // ============= GIZMOS =============
    
    private void OnDrawGizmos()
    {
        if (startPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPoint.position, 0.2f);
        }
        
        if (endPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(endPoint.position, 0.2f);
        }
        
        if (canGrab && Application.isPlaying)
        {
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            foreach (var segment in segments)
            {
                if (!segment.isPinned)
                {
                    Gizmos.DrawWireSphere(segment.position, grabRadius);
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        // Clean up segments
        foreach (var segment in segments)
        {
            if (segment.gameObject != null)
            {
                Destroy(segment.gameObject);
            }
        }
    }
}