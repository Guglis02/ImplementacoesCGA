using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] 
    private TargetCellStategy targetCellStategy;
    [SerializeField]
    private GameObject attackCollider;

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
    private Vector2Int currentCell;

    private CellInterpolator cellInterpolator;

    private float timer = 0;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_Animator = GetComponentInChildren<Animator>();

        m_grid = GameManager.Instance.LevelGrid;
        targetCellStategy.PlaceScatterTargetCell(m_grid.Width, m_grid.Height);

        GameManager.Player.OnPlayerPowerUp += OnPlayerPowerUp;
        GameManager.Player.OnPlayerHit += OnPlayerHit;

        ResetInterpolator();
    }

    private void ResetInterpolator()
    {
        starterCell = currentCell = m_grid.PositionToCoord(transform.position);
        cellInterpolator = new CellInterpolator(starterCell, m_grid, m_characterController, enemyMoveSpeed);
    }

    #region Callbacks and Events
    public void OnEaten()
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
    #endregion

    private void OnDrawGizmos()
    {
        cellInterpolator.Gizmos();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("VolumetricLight"))
        {
            OnEaten();
        }
    }

    private void Update()
    {
        attackCollider.SetActive(false);

        timer += Time.deltaTime;

        switch (behaviourState)
        {
            case BehaviourState.Scatter:
                Scatter();
                break;
            case BehaviourState.Chase:
                Chase();
                break;
            case BehaviourState.Attack:
                Attack();
                break;
            case BehaviourState.Frightened:
                Flee();
                break;
            case BehaviourState.Dead:
                ReturnToSpawn();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        UpdateBehaviour();
    }

    private void Scatter()
    {
        // Se estiver na ghost house, o alvo � sair dela
        if (m_grid[currentCell].levelElementID == LevelBuilder.LevelElementID.Enemy)
        {
            cellInterpolator.SetTargetCell(currentCell + Vector2Int.up);
        }
        else
        {
            cellInterpolator.SetTargetCell(targetCellStategy.GetScatterTargetCell());
        }

        cellInterpolator.Move();
    }

    private void Chase()
    {
        Vector2Int playerCell = m_grid.PositionToCoord(GameManager.PlayerPosition);
        cellInterpolator.SetTargetCell(targetCellStategy.CalculateChaseTargetCell(playerCell, currentCell));

        cellInterpolator.Move();
    }

    private void Attack()
    {
        Vector3 lookDir = transform.position - GameManager.PlayerPosition;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(lookDir.x, 0, lookDir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 20f * Time.deltaTime);
    }

    private void Flee()
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

    private void ReturnToSpawn()
    {
        cellInterpolator.Move();

        if (Vector3.Distance(transform.position, m_grid.CoordToPosition(starterCell)) <= 0.5f)
        {
            behaviourState = BehaviourState.Scatter;
            m_Animator.SetTrigger("Scatter");
            timer = 0;
        }
    }

    private void UpdateBehaviour()
    {
        if (behaviourState == BehaviourState.Dead)
        {
            return;
        } else if (GameManager.Player.IsPoweredUp)
        {
            return;
        } else if (Vector3.Distance(GameManager.PlayerPosition, transform.position) < 1.5f)
        {
            attackCollider.SetActive(true);
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