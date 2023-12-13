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
    private BehaviourState behaviourState = BehaviourState.Scatter;

    private enum BehaviourState
    {
        Scatter,
        Chase,
        Frightened,
        Attack,
        Dead
    }

    private const float enemyMoveSpeed = 1.5f;
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

        m_grid = GameManager.Instance.LevelGrid;
        targetCellStategy.PlaceScatterTargetCell(m_grid.Width, m_grid.Height);

        GameManager.Player.OnPlayerPowerUp += OnPlayerPowerUp;
        GameManager.Player.OnPlayerPowerDown += OnPlayerPowerDown;
        GameManager.Player.OnPlayerHit += OnPlayerHit;

        ResetInterpolator();
    }

    private void ResetInterpolator()
    {
        starterCell = m_grid.PositionToCoord(transform.position);
        cellInterpolator = new CellInterpolator(starterCell, m_grid, m_characterController, enemyMoveSpeed);
    }
    
    public void Die()
    {
        behaviourState = BehaviourState.Dead;
        cellInterpolator.SetTargetCell(starterCell);
        m_Animator.SetTrigger("Dead");
    }

    private void OnPlayerHit(int _)
    {
        m_characterController.SetPosition(m_grid.CoordToPosition(starterCell));
        ResetInterpolator();
        behaviourState = BehaviourState.Scatter;
        m_Animator.SetTrigger("Scatter");
        timer = 0;
    }

    private void OnPlayerPowerUp()
    {
        behaviourState = BehaviourState.Frightened;
    }

    private void OnPlayerPowerDown()
    {
        behaviourState = BehaviourState.Scatter;
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

        switch (behaviourState)
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

        cellInterpolator.Update();
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

        int randomIndex = UnityEngine.Random.Range(0, possibleCells.Count);
        cellInterpolator.SetTargetCell(possibleCells[randomIndex]);
    }

    private void UpdateDeadBehaviour()
    {
        if (Vector3.Distance(transform.position, m_grid.CoordToPosition(starterCell)) <= 0.5f)
        {
            behaviourState = BehaviourState.Scatter;
            m_Animator.SetTrigger("Scatter");
            timer = 0;
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
        if (behaviourState == BehaviourState.Dead 
            || behaviourState == BehaviourState.Frightened)
        {
            return;
        } else if (IsPlayerOnAttackRange(2f))
        {
            behaviourState = BehaviourState.Attack;
            m_Animator.SetTrigger("Attack");
            timer += 20;
        } else if (timer >= 20f && behaviourState != BehaviourState.Scatter)
        {
            behaviourState = BehaviourState.Scatter;
            m_Animator.SetTrigger("Scatter");
            timer = 0;
        }
        else if (timer >= 7f && behaviourState != BehaviourState.Chase)
        {
            behaviourState = BehaviourState.Chase;
            m_Animator.SetTrigger("Chase");
            timer = 0;
        }
    }
}