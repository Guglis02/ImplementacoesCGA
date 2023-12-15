using UnityEngine;

public class SteeringBehaviour : MonoBehaviour
{
    [SerializeField]
    private float m_MaxSpeed = 1.5f;

    [SerializeField]
    private float m_MaxForce = 1f;

    [SerializeField]
    private float m_PanicDistance = 4f;

    public Vector3 m_acceleration = Vector3.zero;

    private CharacterController m_CharacterController;

    private float m_WanderRadius = 2f;
    private float m_WanderTheta = Mathf.PI / 2f;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    public void Seek(Vector3 target)
    {
        Vector3 force = (target - transform.position).normalized * m_MaxSpeed;
        force -= m_CharacterController.velocity;
        force = Vector3.ClampMagnitude(force, m_MaxForce);
        m_acceleration += new Vector3(force.x, 0, force.z);
    }

    public void Flee(Vector3 target)
    {
        if (Vector3.Distance(transform.position, target) > m_PanicDistance)
        {
            return;
        }

        Vector3 force = (transform.position - target).normalized * m_MaxSpeed;
        force -= m_CharacterController.velocity;
        force = Vector3.ClampMagnitude(force, m_MaxForce);
        m_acceleration += new Vector3(force.x, 0, force.z);
    }

    public void Arrive(Vector3 target)
    {
        Vector3 force = (target - transform.position).normalized * m_MaxSpeed;
        float distance = Vector3.Distance(transform.position, target);
        float deceleration = distance / 2f;
        force *= deceleration;
        force -= m_CharacterController.velocity;
        force = Vector3.ClampMagnitude(force, m_MaxForce);
        m_acceleration += new Vector3(force.x, 0, force.z);
    }

    public void Wander()
    {
        Vector3 wanderPoint = m_CharacterController.velocity;
        wanderPoint = Vector3.ClampMagnitude(wanderPoint, 2f);
        wanderPoint += transform.position;

        float theta = m_WanderTheta + Mathf.Atan2(m_CharacterController.velocity.x, m_CharacterController.velocity.z);

        float x = m_WanderRadius * Mathf.Cos(theta);
        float z = m_WanderRadius * Mathf.Sin(theta);

        wanderPoint += new Vector3(x, 0, z);

        Vector3 steer = wanderPoint - transform.position;
        steer = Vector3.ClampMagnitude(steer, m_MaxForce);
        m_acceleration += new Vector3(steer.x, 0, steer.z);

        float displacementRange = 0.3f;
        m_WanderTheta += Random.Range(-displacementRange, displacementRange);
    }

    public void WallAvoidance()
    {
        float rayLength = 1f;
        Vector3 rayVector = m_CharacterController.velocity.normalized;
        rayVector = Vector3.ClampMagnitude(rayVector, rayLength);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, rayVector, out hit, rayLength))
        {
            Vector3 force = hit.normal * m_MaxSpeed;
            force -= m_CharacterController.velocity;
            force = Vector3.ClampMagnitude(force, m_MaxForce);
            m_acceleration += new Vector3(force.x, 0, force.z);
        }
    }

    public void Update()
    {
        //Seek(GameManager.PlayerPosition);
        //Flee(GameManager.PlayerPosition);
        Wander();
        //WallAvoidance();

        Vector3 movementDir = m_acceleration.normalized * m_MaxSpeed * Time.deltaTime;
        m_CharacterController.Move(movementDir);

        Quaternion rotation = Quaternion.LookRotation(-movementDir);
        m_CharacterController.transform.rotation = Quaternion.Slerp(m_CharacterController.transform.rotation,
                                                                rotation,
                                                                5f * Time.deltaTime);
    }
}
