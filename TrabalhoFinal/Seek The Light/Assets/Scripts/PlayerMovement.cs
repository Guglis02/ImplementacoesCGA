using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float m_groundAccelerate = 10;

    [SerializeField]
    private float m_groundFriction = 0.4f;

    [SerializeField]
    private float m_groundMaxSpeed = 10;

    [SerializeField]
    private float m_rotationDamping = 5;

    private Vector3 m_velocity;
    private Vector3 m_previousInputDirection;
    private CharacterController m_characterController;
    private Animator m_animator;

    private Vector3 m_lastFramePos;
    private float m_speed;

    private void Awake()
    {
        m_previousInputDirection = Vector3.forward;
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        m_animator.SetFloat("Speed", m_speed);
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        var inputDirection = new Vector3(horizontal, 0, vertical);
        inputDirection = Vector3.ClampMagnitude(inputDirection, 1);

        if (inputDirection != Vector3.zero)
        {
            m_previousInputDirection = inputDirection;
            m_velocity = inputDirection;
        }

        m_velocity = MoveGround(inputDirection, m_velocity);

        m_characterController.Move(m_velocity * Time.fixedDeltaTime);

        Quaternion rotation = Quaternion.LookRotation(m_previousInputDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * m_rotationDamping);

        var position = transform.position;
        position.y = 1;
        transform.position = position;

        m_speed = (position - m_lastFramePos).magnitude / Time.deltaTime;
        m_lastFramePos = position;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_velocity = ClipVector(m_velocity, hit.normal);
    }

    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        prevVelocity = ApplyFriction(prevVelocity, m_groundFriction);
        return Accelerate(accelDir, prevVelocity, m_groundAccelerate, m_groundMaxSpeed);
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
