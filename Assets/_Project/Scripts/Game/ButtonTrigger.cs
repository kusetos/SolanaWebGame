using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{

    [Header("References")]
    public Transform objectToMove;   // The wall, door, or platform
    public Transform waypoint;       // Target position
    public SpriteRenderer button;
    public Transform point;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public bool returnWhenExit = true; // Should it go back when the object leaves?

    private Vector3 startPos;
    private bool shouldMove = false;
    private bool shouldReturn = false;

    private Color c;
    private Vector3 pos;
    private void Start()
    {
        c = button.color;
        pos = button.transform.position;
        if (objectToMove != null)
            startPos = objectToMove.position;
    }

    private void Update()
    {
        if (objectToMove == null || waypoint == null)
            return;

        if (shouldMove)
        {
            objectToMove.position = Vector3.MoveTowards(
                objectToMove.position,
                waypoint.position,
                moveSpeed * Time.deltaTime
            );
        }
        else if (returnWhenExit && shouldReturn)
        {
            objectToMove.position = Vector3.MoveTowards(
                objectToMove.position,
                startPos,
                moveSpeed * Time.deltaTime
            );
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water")) return;

        shouldMove = true;
        shouldReturn = false;

        if (button != null)
        {
            button.color = new Color(0, 0.5f, 0); // pressed color
            button.transform.position = point.position;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water")) return;

        // Start returning if enabled
        shouldMove = false;
        shouldReturn = true;

        if (button != null)
        {
            button.color = c;
            button.transform.position = pos;
        }
    }
}
