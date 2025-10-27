using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WinTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            LevelManager.Instance.CompleteLevel();
        }   
    }
}
