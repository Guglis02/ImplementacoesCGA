using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector2 previousCell;
    private Vector2 currentCell;
    private Vector2 nextCell;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        previousCell = Vector2.zero;
        nextCell = currentCell = new Vector2(Mathf.RoundToInt(transform.position.x / LevelBuilder.blockSize),
                                             Mathf.RoundToInt(transform.position.z / LevelBuilder.blockSize));
    }

    public void SetInitialTarget(Vector2 targetCell)
    {
        this.targetCell = targetCell;
    }

    private void Update()
    {
        //UpdateBehaviour();
        Debug.DrawLine(transform.position, 
            new Vector3(targetCell.x, 0, targetCell.y) * LevelBuilder.blockSize
            , Color.red);

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
            currentCell = new Vector2((int)((transform.position.x) / LevelBuilder.blockSize),
                                      (int)((transform.position.z) / LevelBuilder.blockSize));
        }
    }

    private void CalculateNextCell()
    {
        float minDistance = float.MaxValue;

        nextCell = previousCell;

        for (int i = 0; i < 4; i++)
        {
            Vector2 direction = Vector2.zero;
            switch (i)
            {
                case 0:
                    direction = Vector2.up;
                    break;
                case 1:
                    direction = Vector2.left;
                    break;
                case 2:
                    direction = Vector2.down;
                    break;
                case 3:
                    direction = Vector2.right;
                    break;
            }

            Vector2 nextCellCandidate = currentCell + direction;
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
