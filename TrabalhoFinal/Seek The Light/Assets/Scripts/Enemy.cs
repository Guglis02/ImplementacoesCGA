using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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

    private CharacterController m_characterController;

    private Grid<LevelBuilder.LevelCell> m_grid;

    private BehaviourState behaviourState = BehaviourState.Scatter;

    private Vector2Int targetCell;
    private Vector2Int previousCell;
    private Vector2Int currentCell;
    private Vector2Int nextCell;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
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
        //UpdateBehaviour();
        UnityEngine.Debug.DrawLine(transform.position, m_grid.CoordToPosition(targetCell), Color.red);

        UnityEngine.Debug.DrawLine(transform.position, m_grid.CoordToPosition(nextCell), Color.green);

        switch (behaviourState)
        {
            case BehaviourState.Scatter:
                Scatter();
                break;
            case BehaviourState.Chase:
                break;
            case BehaviourState.Frightened:
                break;
            case BehaviourState.Dead:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Scatter()
    {
        if (currentCell.Equals(nextCell))
        {
            CalculateNextCell();
        }
        else
        {
            Vector2 dir = nextCell - currentCell;
            Vector3 movementDir = new Vector3(dir.x, 0, dir.y);
            m_characterController.Move(movementDir * Time.fixedDeltaTime);

            previousCell = currentCell;
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
            if (nextCellCandidate.Equals(previousCell)
                || !GameManager.Instance.LevelGrid[nextCellCandidate].isWalkable)
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