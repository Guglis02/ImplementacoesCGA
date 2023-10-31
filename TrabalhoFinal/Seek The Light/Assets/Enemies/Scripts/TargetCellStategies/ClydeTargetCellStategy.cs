using UnityEngine;

[CreateAssetMenu]
public class ClydeTargetCellStategy : TargetCellStategy
{
    public override void PlaceScatterTargetCell(int gridWidth, int gridHeight)
    {
        m_scatterTargetCell = new Vector2Int(0, -1);
    }

    public override Vector2Int CalculateChaseTargetCell()
    {
        Vector2Int playerCell = GameManager.Instance.LevelGrid.PositionToCoord(GameManager.PlayerPosition);

        //TODO: Fazer funcionar igual no jogo original, levando em consideração a posição do Clyde.
        //if (Vector2Int.Distance(playerCell, clydeCell) < 8)
        //{
        //    return m_scatterTargetCell;
        //}

        return playerCell;
    }
}
