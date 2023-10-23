using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraMovement : MonoBehaviour
{
    Bounds m_DeadZone;

    [SerializeField]
    Vector3 m_PlayerOffset;

    [SerializeField]
    float m_CameraRotationDamping = 0.2f;

    [SerializeField]
    float m_CameraPositionDamping = 0.2f;

    private void Awake()
    {
        m_DeadZone = new Bounds(GameManager.PlayerPosition, new Vector3(2 * Screen.width / Screen.height, 0, 2));
    }

    private void Start()
    {
        var playerPosition = GameManager.PlayerPosition;
        m_DeadZone.center += playerPosition;
        transform.position = m_DeadZone.center + m_PlayerOffset;
    }

    private void Update()
    {
        var playerPosition = GameManager.PlayerPosition;
        var closestPointOnDeadZone = m_DeadZone.ClosestPoint(playerPosition);
        m_DeadZone.center += playerPosition - closestPointOnDeadZone;

        transform.position = Vector3.Lerp(transform.position, m_DeadZone.center + m_PlayerOffset, Time.deltaTime * m_CameraPositionDamping);

        Quaternion rotation = Quaternion.LookRotation(playerPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * m_CameraRotationDamping);
    }

    private void OnDrawGizmos()
    {
        var prevColor = Gizmos.color;
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(m_DeadZone.center, m_DeadZone.size);

        Gizmos.color = prevColor;
    }
}
