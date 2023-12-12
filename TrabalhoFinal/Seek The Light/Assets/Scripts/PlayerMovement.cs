using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float m_GroundAccelerate = 10;

    [SerializeField]
    float m_GroundFriction = 0.4f;

    [SerializeField]
    float m_GroundMaxSpeed = 10;

    [SerializeField]
    float m_RotationDamping = 5;

    Vector3 m_Velocity;
    Vector3 m_PreviousInputDirection;
    CharacterController m_CharacterController;
    Animator m_Animator;

    void Awake()
    {
        m_PreviousInputDirection = Vector3.forward;
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        m_Animator.SetFloat("Speed", Vector3.Magnitude(m_Velocity));
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        var inputDirection = new Vector3(horizontal, 0, vertical);
        inputDirection = Vector3.ClampMagnitude(inputDirection, 1);

        if (inputDirection != Vector3.zero)
        {
            m_PreviousInputDirection = inputDirection;
            //m_Velocity = inputDirection;
        }

        m_Velocity = MoveGround(inputDirection, m_Velocity);

        m_CharacterController.Move(m_Velocity * Time.fixedDeltaTime);

        Quaternion rotation = Quaternion.LookRotation(m_PreviousInputDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * m_RotationDamping);

        var position = transform.position;
        position.y = 1;
        transform.position = position;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_Velocity = ClipVector(m_Velocity, hit.normal);
    }

    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        prevVelocity = ApplyFriction(prevVelocity, m_GroundFriction);
        return Accelerate(accelDir, prevVelocity, m_GroundAccelerate, m_GroundMaxSpeed);
    }

    private Vector3 ApplyFriction(Vector3 prevVelocity, float friction)
    {
        float speed = prevVelocity.magnitude;
        if (speed != 0)
        {
            float drop = speed * friction * Time.deltaTime;
            prevVelocity *= Mathf.Max(speed - drop, 0) / speed;
        }

        return prevVelocity;
    }

    private static Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float maxVelocity)
    {
        float projVel = Vector3.Dot(prevVelocity, accelDir);
        float accelVel = accelerate * Time.fixedDeltaTime;

        if (projVel + accelVel > maxVelocity)
            accelVel = maxVelocity - projVel;

        return prevVelocity + accelDir * accelVel;
    }

    private static Vector3 ClipVector(Vector3 vector, Vector3 normal)
    {
        float dotProduct = Mathf.Max(0, Vector3.Dot(vector, normal));
        return vector + dotProduct * normal;
    }
}
