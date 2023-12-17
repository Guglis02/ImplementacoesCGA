using UnityEngine;

[CreateAssetMenu]
public class ClydeTargetCellStategy : TargetCellStategy
{
    public override void PlaceScatterTargetCell(int gridWidth, int gridHeight)
    {
        m_scatterTargetCell = new Vector2Int(0, -1);
    }

    public override Vector2Int CalculateChaseTargetCell(Vector2Int playerCell, Vector2Int enemyCell)
    {
        if (Vector2Int.Distance(playerCell, enemyCell) < 8)
        {
            return m_scatterTargetCell;
        }

        return playerCell;
    }
}
