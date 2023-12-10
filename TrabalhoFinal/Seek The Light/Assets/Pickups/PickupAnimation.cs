using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickupAnimation : MonoBehaviour
{
    [SerializeField]
    float m_MovementSpeed;

    [SerializeField]
    float m_MovementAltitude;

    float m_InitialHegiht;

    private void Start()
    {
        m_InitialHegiht = transform.position.y;
    }

    void Update()
    {
        var position = transform.position;
        position.y = m_InitialHegiht + m_MovementAltitude * Mathf.Sin(Time.time * m_MovementSpeed);
        transform.position = position;
    }
}
