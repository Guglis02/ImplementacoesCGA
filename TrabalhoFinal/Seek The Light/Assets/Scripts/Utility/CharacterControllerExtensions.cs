using UnityEngine;

public static class CharacterControllerExtensions
{
    public static void SetPosition(this CharacterController characterController, Vector3 position)
    {
        bool prevCharacterControlerEnabled = characterController.enabled;

        characterController.enabled = false;
        characterController.transform.position = position;
        characterController.enabled = prevCharacterControlerEnabled;
    }
}
