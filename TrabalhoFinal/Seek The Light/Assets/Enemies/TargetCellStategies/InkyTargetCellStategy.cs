using UnityEngine;

[CreateAssetMenu]
public class InkyTargetCellStategy : TargetCellStategy
{
    private Transform blinkyTransform;

    private Transform BlinkyTransform
    {
        get
        {
            if (blinkyTransform == null)
            {
                blinkyTransform = GameObject.FindGameObjectWithTag("Blinky").gameObject.transform;
            }
            return blinkyTransform;
        }
    }

    public override void PlaceScatterTargetCell(int gridWidth, int gridHeight)
    {
        m_scatterTargetCell = new Vector2Int(gridWidth, -1);
    }

    public override Vector2Int CalculateChaseTargetCell(Vector2Int playerCell, Vector2Int enemyCell)
    {
        Vector2Int blinkyCell = GameManager.Instance.LevelGrid.PositionToCoord(BlinkyTransform.position);

        Vector2Int playerDirection = new Vector2Int((int)GameManager.Player.Forward.x, (int)GameManager.Player.Forward.z);
        Vector2Int advancedCell = playerCell + (playerDirection * 2);
        Vector2Int targetCell = advancedCell + (advancedCell - blinkyCell);

        return targetCell;
    }
}
