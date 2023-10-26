using System;
using System.Diagnostics;
using UnityEngine;

public class Grid<T>
{
    T[,] m_Cells;
    Vector3 m_Center;
    Vector2 m_CellSize;

    public int Width => m_Cells.GetLength(0);
    public int Height => m_Cells.GetLength(1);

    public T this[Vector2Int coord]
    {
        get
        {
            ValidateCoord(coord);
            return m_Cells[coord.x, coord.y];
        }

        set
        {
            ValidateCoord(coord);
            m_Cells[coord.x, coord.y] = value;
        }
    }

    public T this[int x, int y]
    {
        get
        {
            return this[new Vector2Int(x, y)];
        }

        set
        {
            this[new Vector2Int(x, y)] = value;
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
        var relativePosition = position - GetOriginPosition();
        return new
        (
            Mathf.RoundToInt(relativePosition.x / m_CellSize.x),
            Mathf.RoundToInt(relativePosition.z / m_CellSize.y)
        );
    }

    public Vector3 CoordToPosition(Vector2Int coord)
    {
        ValidateCoord(coord);
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

    [Conditional("DEBUG")]
    public void ValidateCoord(Vector2Int coord)
    {
        if (coord.x < 0 || coord.x >= Width)
            throw new ArgumentOutOfRangeException("coord.x");

        if (coord.y < 0 || coord.y >= Height)
            throw new ArgumentOutOfRangeException("coord.y");
    }
}