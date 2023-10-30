using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool IsPoweredUp = false;

    public void SetPosition(Vector3 position)
    {
        var characterController = GetComponent<CharacterController>();
        bool prevCharacterControlerEnabled = characterController.enabled;

        characterController.enabled = false;
        transform.position = position;
        characterController.enabled = prevCharacterControlerEnabled;
    }
}
