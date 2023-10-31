using UnityEngine;

[CreateAssetMenu]
public class InkyTargetCellStategy : TargetCellStategy
{
    public override void PlaceScatterTargetCell(int gridWidth, int gridHeight)
    {
        m_scatterTargetCell = new Vector2Int(gridWidth, -1);
    }

    public override Vector2Int CalculateChaseTargetCell()
    {
        // TODO: Fazer funcionar igual no jogo original, levando em consideração a posição do Blinky.
        return GameManager.Instance.LevelGrid.PositionToCoord(GameManager.PlayerPosition);
    }
}
