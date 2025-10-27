using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KillerBlock : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Health.Instance.TakeDamage(0.2f);
        }
    }
}
