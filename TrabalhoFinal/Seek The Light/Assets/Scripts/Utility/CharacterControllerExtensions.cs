using UnityEngine;

public static class CharacterControllerExtensions
{
    public static void SetPosition(this CharacterController characterController, Vector3 position)
    {
        bool prevActive = characterController.gameObject.activeSelf;

        characterController.gameObject.SetActive(false);
        characterController.transform.position = position;
        characterController.gameObject.SetActive(prevActive);
    }
}
