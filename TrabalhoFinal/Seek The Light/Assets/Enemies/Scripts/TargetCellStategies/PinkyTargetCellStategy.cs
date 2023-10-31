using UnityEngine;

[CreateAssetMenu]
public class PinkyTargetCellStategy : TargetCellStategy
{
    public override void PlaceScatterTargetCell(int gridWidth, int gridHeight)
    {
        m_scatterTargetCell = new Vector2Int(1, gridHeight + 2);
    }

    public override Vector2Int CalculateChaseTargetCell(Vector2Int playerCell, Vector2Int enemyCell)
    {
        Vector2Int playerDirection = new Vector2Int((int)GameManager.Player.Forward.x, (int)GameManager.Player.Forward.z);

        return playerCell + (playerDirection * 4);
    }
}
