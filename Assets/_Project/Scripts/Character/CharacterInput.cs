using System;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{

    // Example input script
    public CharacterManager2D characterManager;
    void Update()
    {
        characterManager.SetMoveInput(Input.GetAxisRaw("Horizontal"));
        // if (Input.GetButtonDown("Jump"))
        //     characterManager.Jump();

        // if (Input.GetButtonUp("Jump"))
        //     characterManager.StopJump();
    }
    

}
