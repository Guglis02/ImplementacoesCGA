using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    Vector3 m_PlayerOffset;

    [SerializeField]
    float m_CameraDistance = 2f;

    [SerializeField]
    float m_CameraRotationDamping = 0.2f;

    [SerializeField]
    float m_CameraPositionDamping = 0.2f;

    private void Start()
    {
        Vector3 playerPosition = GameManager.PlayerPosition;
        transform.position = playerPosition;
    }

    private void Update()
    {
        Vector3 playerPosition = GameManager.PlayerPosition;
        Vector3 playerForward = GameManager.Player.transform.forward;

        transform.position = Vector3.Lerp(transform.position, playerPosition + m_PlayerOffset + playerForward * m_CameraDistance, Time.deltaTime * m_CameraPositionDamping);

        Quaternion rotation = Quaternion.LookRotation(playerPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * m_CameraRotationDamping);
    }
}
