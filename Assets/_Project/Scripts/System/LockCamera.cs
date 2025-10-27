using UnityEngine;
public class LockCamera : MonoBehaviour
{
    private Camera cam;
    //private Transform followTarget;
    private float lockedX;

    void Start()
    {
        cam = Camera.main;

        lockedX = transform.position.x; // store initial X position
    }

    void LateUpdate()
    {

        Vector3 camPos = cam.transform.position;
        camPos.x = lockedX;
        cam.transform.position = camPos;
        
    }
}
