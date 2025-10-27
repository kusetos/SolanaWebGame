using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerInput : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LevelManager.Instance.RestartLevel();
        }
    }
}
 