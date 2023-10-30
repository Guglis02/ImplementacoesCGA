using System;
using UnityEngine;

[SelectionBase]
public class Enemy : MonoBehaviour
{
    private enum BehaviourState
    {
        Scatter,
        Chase,
        Frightened,
        Dead
    }

    private enum EnemyType
    {
        Blinky,
        Pinky,
        Inky,
        Clyde
    }

    private CharacterController m_characterController;
    private Animator m_Animator;

    private Grid<LevelBuilder.LevelCell> m_grid;

    private BehaviourState behaviourState = BehaviourState.Scatter;

    [SerializeField] private Vector2Int targetCell;

    private Vector2Int previousCell;
    private Vector2Int currentCell;
    private Vector2Int nextCell;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_Animator = GetComponentInChildren<Animator>();
        m_grid = GameManager.Instance.LevelGrid;
        previousCell = Vector2Int.zero;
        nextCell = currentCell = m_grid.PositionToCoord(transform.position);
    }

    public void SetInitialTarget(Vector2Int targetCell)
    {
        this.targetCell = targetCell;
    }

    private void Update()
    {
        UpdateBehaviour();
        
        Debug.DrawLine(transform.position, m_grid.CoordToPosition(targetCell), Color.red);
        Debug.DrawLine(transform.position, m_grid.CoordToPosition(nextCell), Color.green);

        switch (behaviourState)
        {
            case BehaviourState.Scatter:
                scatterTimer += Time.deltaTime;
                Move();
                break;
            case BehaviourState.Chase:
                chaseTimer += Time.deltaTime;
                Move();
                break;
            case BehaviourState.Frightened:
                break;
            case BehaviourState.Dead:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    float scatterTimer = 0f;
    float chaseTimer = 0f;

    private void UpdateBehaviour()
    {
        if (scatterTimer >= 7f)
        {
            behaviourState = BehaviourState.Chase;
            scatterTimer = 0;
        } else if (chaseTimer >= 20f)
        {
            behaviourState = BehaviourState.Scatter;
            chaseTimer = 0;
        }

    }

    Vector3 movementDir = Vector3.zero;
    private void Move()
    {
        if (Vector3.Distance(transform.position, m_grid.CoordToPosition(nextCell)) <= 3.5f)
        {
            CalculateNextCell();
            Vector2 dir = nextCell - currentCell;
            movementDir = new Vector3(dir.x, 0, dir.y);
        }
        else
        {
            m_characterController.Move(movementDir * Time.fixedDeltaTime);

            if (!currentCell.Equals(nextCell))
            {
                previousCell = currentCell;
            }
            currentCell = m_grid.PositionToCoord(transform.position);
        }
    }

    private void CalculateNextCell()
    {
        float minDistance = float.MaxValue;

        nextCell = previousCell;

        for (int i = 0; i < 4; i++)
        {
            Vector2Int direction = Vector2Int.zero;
            switch (i)
            {
                case 0:
                    direction = Vector2Int.up;
                    break;
                case 1:
                    direction = Vector2Int.left;
                    break;
                case 2:
                    direction = Vector2Int.down;
                    break;
                case 3:
                    direction = Vector2Int.right;
                    break;
            }

            Vector2Int nextCellCandidate = currentCell + direction;

            if (i == 0 && m_grid[nextCellCandidate].color == Color.cyan)
            {
                nextCell = nextCellCandidate;
                break;
            }

            if (nextCellCandidate.Equals(previousCell)
                || !m_grid[nextCellCandidate].isWalkable)
            {
                continue;
            }

            float distance = Vector2.Distance(nextCellCandidate, targetCell);
            if (distance < minDistance)
            {
                minDistance = distance;
                nextCell = nextCellCandidate;
            }
        }
    }
}