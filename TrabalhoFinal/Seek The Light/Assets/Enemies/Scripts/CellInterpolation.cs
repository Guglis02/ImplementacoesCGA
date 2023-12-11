using UnityEngine;

public class CellInterpolator
{
    private Vector3 movementDir = Vector3.zero;
    private Vector2Int currentCell;
    private Vector2Int previousCell;
    private Vector2Int nextCell;
    private Vector2Int targetCell;
    private Grid<LevelBuilder.LevelCell> grid;
    private CharacterController characterController;
    private float moveSpeed;

    public CellInterpolator(Vector2Int starterCell,
                            Grid<LevelBuilder.LevelCell> grid,
                            CharacterController characterController, 
                            float moveSpeed)
    {
        currentCell = starterCell;
        previousCell = starterCell;
        nextCell = starterCell;
        this.grid = grid;
        this.characterController = characterController;
        this.moveSpeed = moveSpeed;
    }

    public void Gizmos()
    {
        Debug.DrawLine(characterController.transform.position,
            grid.CoordToPosition(targetCell), Color.red);
        Debug.DrawLine(characterController.transform.position,
            grid.CoordToPosition(nextCell), Color.green);
    }

    public Vector2Int GetCurrentCell()
    {
        return currentCell;
    }

    public void SetTargetCell(Vector2Int targetCell)
    {
        this.targetCell = targetCell;
    }

    public void Move()
    {
        if (Vector3.Distance(characterController.transform.position, grid.CoordToPosition(nextCell)) <= 0.5f)
        {
            CalculateNextCell();
            Vector2 dir = nextCell - currentCell;
            movementDir = new Vector3(dir.x, 0, dir.y);
        }
        else
        {
            characterController.Move(movementDir * moveSpeed * Time.deltaTime);

            Quaternion rotation = Quaternion.LookRotation(-movementDir);
            characterController.transform.rotation = Quaternion.Slerp(characterController.transform.rotation,
                                                                    rotation,
                                                                    5f * Time.deltaTime);

            if (!currentCell.Equals(nextCell))
            {
                previousCell = currentCell;
            }
            currentCell = grid.PositionToCoord(characterController.transform.position);
        }
    }

    public bool TryGetNeighbourCell(int index, out Vector2Int cell)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.down,
            Vector2Int.right
        };

        cell = currentCell + directions[index];

        return !cell.Equals(previousCell) && grid[cell].isWalkable;
    }

    private void CalculateNextCell()
    {
        float minDistance = float.MaxValue;

        nextCell = previousCell;

        for (int i = 0; i < 4; i++)
        {
            if (!TryGetNeighbourCell(i, out Vector2Int nextCellCandidate))
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