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

    private BehaviourState behaviourState = BehaviourState.Scatter;
    private Vector2 targetCell;

    private Vector2Int previousCell;
    private Vector2Int currentCell;
    private Vector2Int nextCell;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        previousCell = Vector2Int.zero;
        nextCell = currentCell = new Vector2Int(Mathf.RoundToInt(transform.position.x / LevelBuilder.blockSize),
                                             Mathf.RoundToInt(transform.position.z / LevelBuilder.blockSize));
    }

    public void SetInitialTarget(Vector2 targetCell)
    {
        this.targetCell = targetCell;
    }

    private void Update()
    {
        //UpdateBehaviour();
        UnityEngine.Debug.DrawLine(transform.position, 
            new Vector3(targetCell.x, 0, targetCell.y) * LevelBuilder.blockSize
            , Color.red);

        UnityEngine.Debug.DrawLine(transform.position,
            new Vector3(nextCell.x, 0, nextCell.y) * LevelBuilder.blockSize
                       , Color.green);

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
            Vector3 movementDirection = new Vector3(
                (nextCell.x - currentCell.x),
                0,
                (nextCell.y - currentCell.y));
            m_characterController.Move(movementDirection * Time.fixedDeltaTime);

            previousCell = currentCell;
            currentCell = Vector2Int.RoundToInt(new Vector2(((transform.position.x) / LevelBuilder.blockSize), ((transform.position.z) / LevelBuilder.blockSize)));
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
                || !GameManager.Instance.WalkableCells.Contains(nextCellCandidate))
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

    private void UpdateBehaviour()
    {
        throw new NotImplementedException();
    }
}