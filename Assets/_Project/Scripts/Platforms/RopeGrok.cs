using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeGrok : MonoBehaviour
{
    [Header("Rope Settings")]
    [SerializeField] private GameObject segmentPrefab; // Rope segment prefab
    [SerializeField] private int segmentCount = 10; // Number of segments
    [SerializeField] private float segmentLength = 0.5f; // Distance between segments
    [SerializeField] private float segmentMass = 0.1f; // Mass of each segment
    [SerializeField] private float jointAngleLimit = 45f; // Angle limit for HingeJoint2D
    [SerializeField] private float ropeDamping = 0.5f; // Damping for smoother motion
    [SerializeField] private bool attachToStartPoint = true; // Anchor the first segment
    [SerializeField] private Transform startPoint; // Optional: Fixed point to attach rope

    [Header("Collision Settings")]
    [SerializeField] private LayerMask collisionLayer; // Layers the rope can collide with
    [SerializeField] private float segmentColliderSize = 0.2f; // Size of segment colliders

    private GameObject[] segments; // Array to store rope segments
    private Rigidbody2D lastSegmentRigidbody; // For attaching objects to the end

    private void Start()
    {
        GenerateRope();
    }

    private void GenerateRope()
    {
        segments = new GameObject[segmentCount];
        Vector2 currentPos = startPoint != null ? startPoint.position : transform.position;
        Rigidbody2D prevRigidbody = null;

        for (int i = 0; i < segmentCount; i++)
        {
            // Instantiate segment
            GameObject segment = Instantiate(segmentPrefab, currentPos, Quaternion.identity, transform);
            segments[i] = segment;

            // Configure Rigidbody2D
            Rigidbody2D rb = segment.GetComponent<Rigidbody2D>();
            rb.mass = segmentMass;
            rb.linearDamping = ropeDamping;

            // Configure Collider
            Collider2D col = segment.GetComponent<Collider2D>();
            if (col is CircleCollider2D circleCol)
                circleCol.radius = segmentColliderSize;
            else if (col is BoxCollider2D boxCol)
                boxCol.size = new Vector2(segmentColliderSize, segmentColliderSize);

            // Configure HingeJoint2D
            HingeJoint2D hinge = segment.GetComponent<HingeJoint2D>();
            if (i == 0 && attachToStartPoint)
            {
                // Anchor first segment to start point
                if (startPoint != null)
                {
                    hinge.connectedBody = startPoint.GetComponent<Rigidbody2D>();
                    hinge.connectedAnchor = Vector2.zero;
                }
                else
                {
                    rb.bodyType = RigidbodyType2D.Static; // Static if no start point
                }
            }
            else
            {
                // Connect to previous segment
                hinge.connectedBody = prevRigidbody;
                hinge.connectedAnchor = Vector2.up * segmentLength;
                hinge.anchor = Vector2.down * segmentLength * 0.5f;

                // Apply angle limits
                if (jointAngleLimit > 0)
                {
                    hinge.useLimits = true;
                    JointAngleLimits2D limits = hinge.limits;
                    limits.min = -jointAngleLimit;
                    limits.max = jointAngleLimit;
                    hinge.limits = limits;
                }
            }

            // Update for next iteration
            prevRigidbody = rb;
            lastSegmentRigidbody = rb;
            currentPos += Vector2.down * segmentLength;
        }
    }

    // Attach an object to the end of the rope
    public void AttachToEnd(GameObject target)
    {
        if (lastSegmentRigidbody == null) return;

        HingeJoint2D hinge = target.AddComponent<HingeJoint2D>();
        hinge.connectedBody = lastSegmentRigidbody;
        hinge.connectedAnchor = Vector2.down * segmentLength * 0.5f;
        hinge.anchor = Vector2.zero;
    }

    // Detach the rope from its start point (make it dynamic)
    public void Detach()
    {
        if (segments.Length > 0)
        {
            Rigidbody2D firstSegmentRb = segments[0].GetComponent<Rigidbody2D>();
            firstSegmentRb.bodyType = RigidbodyType2D.Dynamic;
            HingeJoint2D hinge = segments[0].GetComponent<HingeJoint2D>();
            hinge.connectedBody = null;
        }
    }

    // Visualize rope connections in editor
    private void OnDrawGizmos()
    {
        if (segments == null || segments.Length == 0) return;
        Gizmos.color = Color.green;
        for (int i = 0; i < segments.Length - 1; i++)
        {
            if (segments[i] != null && segments[i + 1] != null)
            {
                Gizmos.DrawLine(segments[i].transform.position, segments[i + 1].transform.position);
            }
        }
    }
}