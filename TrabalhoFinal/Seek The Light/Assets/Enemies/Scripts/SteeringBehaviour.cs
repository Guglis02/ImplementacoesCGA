using UnityEngine;

public class SteeringBehaviour : MonoBehaviour
{
    [SerializeField]
    private float m_MaxForce = 1f;

    [SerializeField]
    private float m_PanicDistance = 4f;

    [SerializeField]
    private float m_WanderAngleVariance;

    public Vector3 m_acceleration = Vector3.zero;

    public float m_MaxSpeed;

    private CharacterController m_CharacterController;

    private float m_WanderDistance = 2f;
    private float m_WanderRadius = 2f;
    private float m_WanderTheta = Mathf.PI / 2f;
    private float m_WallDetectionDistance = 1f;

    public bool m_ShouldFlee = false;

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

    public Vector3 UpdateWanderPoint()
    {
        float theta = m_WanderTheta + Mathf.Atan2(m_CharacterController.velocity.x, 
                                                  m_CharacterController.velocity.z);

        Vector3 wanderPoint = m_CharacterController.velocity;
        wanderPoint = Vector3.ClampMagnitude(wanderPoint, m_WanderDistance);
        wanderPoint += transform.position;

        float x = m_WanderRadius * Mathf.Cos(theta);
        float z = m_WanderRadius * Mathf.Sin(theta);

        wanderPoint += new Vector3(x, 0, z);

        float displacementRange = m_WanderAngleVariance * Time.deltaTime;
        m_WanderTheta += Random.Range(-displacementRange, displacementRange);

        return wanderPoint;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying
            || GameManager.Instance.aiMode != GameManager.AiMode.SteeringBehaviour)
        {
            return;
        }

        Vector3 forwardRayVector = m_CharacterController.velocity.normalized;

        forwardRayVector = Vector3.ClampMagnitude(forwardRayVector, m_WallDetectionDistance);

        Gizmos.DrawRay(transform.position, forwardRayVector);

        Gizmos.color = Color.blue;
        Vector3 wanderPoint = m_CharacterController.velocity;
        wanderPoint = Vector3.ClampMagnitude(wanderPoint, m_WanderDistance);
        wanderPoint += transform.position;
        Gizmos.DrawWireSphere(wanderPoint, m_WanderRadius);

        wanderPoint = UpdateWanderPoint();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wanderPoint, 0.5f);
    }

    public void WallAvoidance()
    {
        Vector3 forwardRayVector = m_CharacterController.velocity.normalized;

        forwardRayVector = Vector3.ClampMagnitude(forwardRayVector, m_WallDetectionDistance);

        int wallLayer = LayerMask.GetMask("Wall");

        if (Physics.Raycast(transform.position, forwardRayVector, out RaycastHit forwardHit, m_WallDetectionDistance, wallLayer))
        {
            Vector3 force = forwardHit.normal * m_MaxSpeed;
            force -= m_CharacterController.velocity.normalized;
            force = Vector3.ClampMagnitude(force, m_MaxForce);
            m_acceleration += new Vector3(force.x, 0, force.z);
        }
    }

    public void UpdateSteering(Vector3 target)
    {        
        if (m_ShouldFlee)
        {
            Flee(GameManager.PlayerPosition);
        }
        else
        {
            Seek(target);
        }
        WallAvoidance();

        m_acceleration = Vector3.ClampMagnitude(m_acceleration, m_MaxForce);
        Vector3 movementDir = m_acceleration * m_MaxSpeed * Time.deltaTime;
        m_CharacterController.Move(movementDir);

        Quaternion rotation = Quaternion.LookRotation(-movementDir);
        m_CharacterController.transform.rotation = Quaternion.Slerp(m_CharacterController.transform.rotation,
                                                                rotation,
                                                                5f * Time.deltaTime);
    }
}
