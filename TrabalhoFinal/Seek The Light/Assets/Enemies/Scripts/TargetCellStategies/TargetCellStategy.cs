using UnityEngine;

public abstract class TargetCellStategy : ScriptableObject
{
    protected Vector2Int m_scatterTargetCell;

    public Vector2Int GetScatterTargetCell()
    {
        return m_scatterTargetCell;
    }

    public abstract void PlaceScatterTargetCell(int gridWidth, int gridHeight);

    public abstract Vector2Int CalculateChaseTargetCell(Vector2Int playerCell, Vector2Int enemyCell);
}
