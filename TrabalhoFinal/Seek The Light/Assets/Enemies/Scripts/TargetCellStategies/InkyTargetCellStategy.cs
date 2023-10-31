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
        // TODO: Fazer funcionar igual no jogo original, levando em considera��o a posi��o do Blinky.
        return GameManager.Instance.LevelGrid.PositionToCoord(GameManager.PlayerPosition);
    }
}
