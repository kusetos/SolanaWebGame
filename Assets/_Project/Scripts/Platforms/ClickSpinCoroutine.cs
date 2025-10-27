using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class ClickSpinCoroutine : MonoBehaviour
{
    public UnityEvent action;

    [Header("Spin Settings")]
    public Vector3 rotationAxis = Vector3.forward; // Z axis for 2D
    public float rotationSpeed = 360f;             // degrees per second
    public float spinDuration = 2f;                // how long it spins
    //public bool randomDirection = false;           // optional: randomize spin direction

    private bool isSpinning = false;

    void OnMouseDown()
    {
        if (!isSpinning)
        {
            StartCoroutine(Spin());
            action?.Invoke();
        }
        
    }

    private IEnumerator Spin()
    {
        isSpinning = true;

        //float direction = randomDirection && UnityEngine.Random.value > 0.5f ? -1f : 1f;
        float timer = 0f;

        while (timer < spinDuration)
        {
            transform.Rotate(rotationAxis * rotationSpeed  * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isSpinning = false;
    }
}
