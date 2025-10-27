using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeBuilder : MonoBehaviour
{
    [Header("Rope Settings")]
    public Transform startPoint;             // attach here (optional)
    public Transform endPoint;               // optional (if you want fixed end)
    public GameObject segmentPrefab;         // RopeSegment prefab
    public int segmentCount = 12;            // number of segments
    public float segmentLength = 0.2f;       // distance between segment centers
    public bool attachStartToAnchor = true;  // connect first segment to startPoint
    public bool attachEndToAnchor = false;   // connect last segment to endPoint
    public float lineWidth = 0.05f;

    private List<GameObject> segments = new List<GameObject>();
    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
        lr.startWidth = lr.endWidth = lineWidth;
        BuildRope();
    }

    public void BuildRope()
    {
        ClearRope();

        if (segmentPrefab == null)
        {
            Debug.LogError("RopeBuilder: segmentPrefab is null");
            return;
        }

        Vector3 startPos = startPoint ? startPoint.position : transform.position;
        Vector3 endPos = endPoint ? endPoint.position : startPos + transform.up * (segmentCount * segmentLength);

        // Build linear chain between startPos and endPos
        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / Mathf.Max(1, segmentCount - 1);
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);

            GameObject seg = Instantiate(segmentPrefab, pos, Quaternion.identity, transform);
            seg.name = "RopeSegment_" + i;

            Rigidbody2D rb = seg.GetComponent<Rigidbody2D>();
            HingeJoint2D hj = seg.GetComponent<HingeJoint2D>();
            if (hj == null) hj = seg.AddComponent<HingeJoint2D>();

            // Connect joint to previous segment's Rigidbody
            if (i > 0)
            {
                Rigidbody2D prevRb = segments[i - 1].GetComponent<Rigidbody2D>();
                hj.connectedBody = prevRb;
                // set anchor/connectedAnchor optionally to center
            }
            else
            {
                // first segment
                if (attachStartToAnchor && startPoint != null)
                {
                    // connect first segment to a static anchor at startPoint
                    GameObject anchor = new GameObject("RopeAnchor_Start");
                    anchor.transform.position = startPoint.position;
                    Rigidbody2D anchorRb = anchor.AddComponent<Rigidbody2D>();
                    anchorRb.bodyType = RigidbodyType2D.Static;
                    hj.connectedBody = anchorRb;
                    anchor.transform.parent = transform;
                }
            }

            segments.Add(seg);
        }

        // Attach end if requested
        if (attachEndToAnchor && endPoint != null && segments.Count > 0)
        {
            var lastHj = segments[segments.Count - 1].GetComponent<HingeJoint2D>();
            GameObject anchor = new GameObject("RopeAnchor_End");
            anchor.transform.position = endPoint.position;
            Rigidbody2D anchorRb = anchor.AddComponent<Rigidbody2D>();
            anchorRb.bodyType = RigidbodyType2D.Static;
            lastHj.connectedBody = anchorRb;
            anchor.transform.parent = transform;
        }

        // Setup line renderer
        lr.positionCount = segments.Count;
    }

    void Update()
    {
        if (segments == null || segments.Count == 0) return;

        // Update line renderer positions to segment positions
        for (int i = 0; i < segments.Count; i++)
        {
            lr.SetPosition(i, segments[i].transform.position);
        }
    }

    public Transform GetNearestSegment(Vector3 worldPos, out int index)
    {
        index = -1;
        float best = float.MaxValue;
        Transform bestT = null;
        for (int i = 0; i < segments.Count; i++)
        {
            float d = Vector2.SqrMagnitude((Vector2)(segments[i].transform.position - worldPos));
            if (d < best)
            {
                best = d;
                bestT = segments[i].transform;
                index = i;
            }
        }
        return bestT;
    }

    public GameObject GetSegment(int index)
    {
        if (index >= 0 && index < segments.Count) return segments[index];
        return null;
    }

    public void ClearRope()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        segments.Clear();
        lr.positionCount = 0;
    }

    // Optional helper: attach a Rigidbody2D to a segment (creates FixedJoint2D)
    public FixedJoint2D AttachRigidbodyToSegment(Rigidbody2D body, int segmentIndex)
    {
        GameObject seg = GetSegment(segmentIndex);
        if (seg == null) return null;

        FixedJoint2D fj = seg.AddComponent<FixedJoint2D>();
        fj.connectedBody = body;
        return fj;
    }
}
