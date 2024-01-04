using UnityEngine;

public class PickupAnimation : MonoBehaviour
{
    [SerializeField]
    private float m_movementSpeed;
    [SerializeField]
    private float m_movementAltitude;
    private float m_initialHegiht;

    private void Start()
    {
        m_initialHegiht = transform.position.y;
    }

    private void Update()
    {
        var position = transform.position;
        position.y = m_initialHegiht + m_movementAltitude * Mathf.Sin(Time.time * m_movementSpeed);
        transform.position = position;
    }
}
