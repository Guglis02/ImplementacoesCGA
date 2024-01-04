using UnityEngine;

[CreateAssetMenu]
public class BlinkyTargetCellStategy : TargetCellStategy
{
    public override void PlaceScatterTargetCell(int gridWidth, int gridHeight)
    {
        m_scatterTargetCell = new Vector2Int(gridWidth - 1, gridHeight + 2);
    }

    public override Vector2Int CalculateChaseTargetCell(Vector2Int playerCell, Vector2Int enemyCell)
    {
        return playerCell;
    }
}
