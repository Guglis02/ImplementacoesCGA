using UnityEngine;

public class Grid<T>
{
    private T[,] m_Cells;
    private Vector3 m_Center;
    private Vector2 m_CellSize;

    public Vector2 CellSize => m_CellSize;
    public int Width => m_Cells.GetLength(0);
    public int Height => m_Cells.GetLength(1);

    public ref T this[Vector2Int coord]
    {
        get
        {
            return ref m_Cells[coord.x, coord.y];
        }
    }

    public ref T this[int x, int y]
    {
        get
        {
            return ref this[new Vector2Int(x, y)];
        }
    }

    public Grid(T[,] cells, Vector3 center, Vector2 cellSize)
    {
        m_Cells = cells;
        m_Center = center;
        m_CellSize = cellSize;
    }

    public Vector2Int PositionToCoord(Vector3 position)
    {
        Vector3 relativePosition = position - GetOriginPosition();
        return new
        (
            Mathf.FloorToInt(relativePosition.x / m_CellSize.x),
            Mathf.FloorToInt(relativePosition.z / m_CellSize.y)
        );
    }

    public Vector3 CoordToPosition(float x, float y)
    {
        return GetOriginPosition() + new Vector3(m_CellSize.x * (x + 0.5f), 0, m_CellSize.y * (y + 0.5f));
    }

    public Vector3 CoordToPosition(Vector2Int coord)
    {
        return GetOriginPosition() + new Vector3(m_CellSize.x * (coord.x + 0.5f), 0, m_CellSize.y * (coord.y + 0.5f));
    }

    public Vector3 GridTotalSize()
    {
        return new(Width * m_CellSize.x, 0, Height * m_CellSize.y);
    }

    public Vector3 GetOriginPosition()
    {
        return m_Center - GridTotalSize() / 2;
    }
}