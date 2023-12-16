using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private TargetCellStategy targetCellStategy;

    [SerializeField]
    private BehaviourState m_CurrentBehaviourState;

    [SerializeField]
    private float m_MoveSpeed = 1.5f;

    private BehaviourState CurrentBehaviourState
    {
        get {
            return m_CurrentBehaviourState;
        }
        set {
            if (m_CurrentBehaviourState == value)
            {
                return;
            }

            m_CurrentBehaviourState = value;
            m_Animator.SetTrigger(m_CurrentBehaviourState.ToString());
        }
    }

    private EnemyMeshController m_EnemyMeshController;
    private SteeringBehaviour m_SteeringBehaviour;

    private enum BehaviourState
    {
        Scatter,
        Chase,
        Frightened,
        Attack,
        Dead
    }


    private const float AttackRange = 2f;

    private CharacterController m_characterController;
    private Animator m_Animator;

    private Grid<LevelBuilder.LevelCell> m_grid;

    private Vector2Int starterCell;

    private CellInterpolator cellInterpolator;

    private float timer = 0;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_Animator = GetComponentInChildren<Animator>();
        m_EnemyMeshController = GetComponentInChildren<EnemyMeshController>();
        m_SteeringBehaviour = GetComponent<SteeringBehaviour>();
        m_SteeringBehaviour.m_MaxSpeed = m_MoveSpeed;

        m_grid = GameManager.Instance.LevelGrid;
        targetCellStategy.PlaceScatterTargetCell(m_grid.Width, m_grid.Height);

        GameManager.Player.OnPlayerPowerUp += OnPlayerPowerUp;
        GameManager.Player.OnPlayerPowerDown += OnPlayerPowerDown;
        GameManager.Player.OnPlayerHit += OnPlayerHit;

        ResetInterpolator();
    }

    private void Start()
    {
        CurrentBehaviourState = BehaviourState.Scatter;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CurrentBehaviourState == BehaviourState.Dead)
        {
            return;
        }

        if (other.gameObject.CompareTag("VolumetricLight"))
        {
            Die();
        }
    }

    private void ResetInterpolator()
    {
        starterCell = m_grid.PositionToCoord(transform.position);
        cellInterpolator = new CellInterpolator(starterCell, m_grid, m_characterController, m_MoveSpeed);
    }
    
    public void Die()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
        CurrentBehaviourState = BehaviourState.Dead;
        cellInterpolator.SetTargetCell(starterCell);
        m_EnemyMeshController.SetScorchedMaterials();
    }

    private void Revive()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), false);
        m_EnemyMeshController.SetDefaultMaterials();
        CurrentBehaviourState = BehaviourState.Scatter;
        timer = 0;
    }

    private void OnPlayerHit(int _)
    {
        m_characterController.SetPosition(m_grid.CoordToPosition(starterCell));
        ResetInterpolator();
        Revive();
    }

    private void OnPlayerPowerUp()
    {
        CurrentBehaviourState = BehaviourState.Frightened;
        m_SteeringBehaviour.m_ShouldFlee = true;
    }

    private void OnPlayerPowerDown()
    {
        CurrentBehaviourState = BehaviourState.Scatter;
        m_SteeringBehaviour.m_ShouldFlee = false;
        timer = 0;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        cellInterpolator?.Gizmos();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        switch (CurrentBehaviourState)
        {
            case BehaviourState.Scatter:
                UpdateScatterBehaviour();
                break;
            case BehaviourState.Chase:
                UpdateChaseBehaviour();
                break;
            case BehaviourState.Attack:
                UpdateAttackBehaviour();
                break;
            case BehaviourState.Frightened:
                UpdateFleeBehaviour();
                break;
            case BehaviourState.Dead:
                UpdateDeadBehaviour();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (GameManager.Instance.aiMode)
        {
            case GameManager.AiMode.GridInterpolation:
                cellInterpolator.Update();
                break;
            case GameManager.AiMode.SteeringBehaviour:
                m_SteeringBehaviour.UpdateSteering(cellInterpolator.GetTargetPos());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        UpdateBehaviour();
    }

    private void UpdateScatterBehaviour()
    {
        Vector2Int currentCell = cellInterpolator.GetCurrentCell();

        // Se estiver na ghost house, o alvo eh sair dela
        if (m_grid[currentCell].levelElementID == LevelBuilder.LevelElementID.Enemy)
        {
            cellInterpolator.SetTargetCell(currentCell + Vector2Int.up);
        }
        else
        {
            cellInterpolator.SetTargetCell(targetCellStategy.GetScatterTargetCell());
        }
    }

    private void UpdateChaseBehaviour()
    {
        Vector2Int playerCell = m_grid.PositionToCoord(GameManager.PlayerPosition);
        Vector2Int currentCell = cellInterpolator.GetCurrentCell();

        cellInterpolator.SetTargetCell(targetCellStategy.CalculateChaseTargetCell(playerCell, currentCell));
    }

    private void UpdateAttackBehaviour()
    {
        Vector3 lookDir = transform.position - GameManager.PlayerPosition;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(lookDir.x, 0, lookDir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 20f * Time.deltaTime);
    }

    private void UpdateFleeBehaviour()
    {
        List<Vector2Int> possibleCells = new List<Vector2Int>();

        for (int index = 0; index < 4; index++)
        {
            Vector2Int nextCellCandidate;

            if (!cellInterpolator.TryGetNeighbourCell(index, out nextCellCandidate))
            {
                continue;
            }

            possibleCells.Add(nextCellCandidate);
        }

        int randomIndex = UnityEngine.Random.Range(0, possibleCells.Count - 1);
        cellInterpolator.SetTargetCell(possibleCells[randomIndex]);
    }

    private void UpdateDeadBehaviour()
    {
        if (Vector3.Distance(transform.position, m_grid.CoordToPosition(starterCell)) <= 0.5f)
        {
            Revive();
        }
    }

    bool IsPlayerOnAttackRange(float attackRange)
    {
        Vector3 playerPosition = GameManager.PlayerPosition;
        Vector2 player2dPosition = new Vector2(playerPosition.x, playerPosition.z);
        Vector2 current2dPosition = new Vector2(transform.position.x, transform.position.z);

        float distanceToPlayer = Vector2.Distance(player2dPosition, current2dPosition);

        return distanceToPlayer < attackRange;
    }

    private void UpdateBehaviour()
    {
        if (CurrentBehaviourState == BehaviourState.Dead 
            || CurrentBehaviourState == BehaviourState.Frightened)
        {
            return;
        } else if (IsPlayerOnAttackRange(AttackRange))
        {
            CurrentBehaviourState = BehaviourState.Attack;
            timer += 20;
        } else if (timer >= 20f && CurrentBehaviourState != BehaviourState.Scatter)
        {
            CurrentBehaviourState = BehaviourState.Scatter;
            timer = 0;
        }
        else if (timer >= 7f && CurrentBehaviourState != BehaviourState.Chase)
        {
            CurrentBehaviourState = BehaviourState.Chase;
            timer = 0;
        }
    }
}