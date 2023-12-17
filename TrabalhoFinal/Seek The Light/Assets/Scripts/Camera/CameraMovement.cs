using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_playerOffset;

    [SerializeField]
    private float m_cameraDistance = 2f;

    [SerializeField]
    private float m_cameraRotationDamping = 0.2f;

    [SerializeField]
    private float m_cameraPositionDamping = 0.2f;

    private Vector3 m_playerPosition;

    private void Start()
    {
        m_playerPosition = GameManager.PlayerPosition;
        transform.position = m_playerPosition;
    }

    private void Update()
    {
        m_playerPosition = GameManager.PlayerPosition;
        Vector3 playerForward = GameManager.Player.transform.forward;

        transform.position = Vector3.Lerp(
            transform.position,
            m_playerPosition + m_playerOffset + playerForward * m_cameraDistance,
            Time.deltaTime * m_cameraPositionDamping);

        Quaternion rotation = Quaternion.LookRotation(m_playerPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * m_cameraRotationDamping);
    }
}
