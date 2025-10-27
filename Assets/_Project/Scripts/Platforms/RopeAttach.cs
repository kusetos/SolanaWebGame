using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeAttach : MonoBehaviour
{
    public RopeBuilder rope;              // drag the RopeBuilder in inspector
    public float attachRadius = 1f;
    private FixedJoint2D joint;

    public bool IsAttached => joint != null;

    public void AttachToNearest()
    {
        if (rope == null) return;
        if (IsAttached) return;

        int idx;
        Transform seg = rope.GetNearestSegment(transform.position, out idx);
        if (seg == null) return;

        float dist = Vector2.Distance(seg.position, transform.position);
        if (dist <= attachRadius)
        {
            // create FixedJoint2D on the rope segment, connect to this rigidbody
            Rigidbody2D myRb = GetComponent<Rigidbody2D>();
            joint = seg.gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = myRb;
            joint.enableCollision = false;
            // optional: tune joint params
            Debug.Log($"Attached {gameObject.name} to rope segment {idx}");
        }
    }

    public void Detach()
    {
        if (joint == null) return;
        Destroy(joint);
        joint = null;
    }

    // Auto attach if entering trigger of rope segment (optional)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if you want to auto-attach when touching rope segments:
        if (collision.collider.CompareTag("Rope"))
        {
            AttachToNearest();
        }
    }
}
