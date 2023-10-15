using System;
using System.Collections.Generic;
using UnityEngine;

public class SpatialHash
{
    private Vector2 cellDimension;
    private int cellAmount;
    private int horizontalCells = 5;
    private int verticalCells = 5;

    private List<Line> lines = new List<Line>();
    private int[] used;
    private int[] initial;
    private int[] final;
    private int[] objectIndex;
    private Line[] hashTable;

    public SpatialHash(int planeWidth, int planeHeight)
    {
        this.cellDimension = new Vector2(planeWidth / horizontalCells, planeHeight / verticalCells);
        this.cellAmount = horizontalCells * verticalCells;
    }

    public void SetLines(List<Line> lines)
    {
        this.lines = lines;
        StartHashTable();
        SpatialHashingUpdate();
    }

    public void CheckHashIntersection(Circle circle)
    {
        int cellIndex = HashFunction(circle.transform.position);

        if (used[cellIndex] == 0)
        {
            return;
        }

        for (int i = initial[cellIndex]; i < initial[cellIndex] + used[cellIndex]; i++)
        {
            Line line = hashTable[i];

            if (UtilitaryMethods.LineCircleIntersection(line, circle))
            {
                circle.Collide(true);
                return;
            }
        }
    }

    private int HashFunction(Vector2 point)
    {
        Vector2 pointInGrid = point / cellDimension;

        int x = (int)Mathf.Clamp(pointInGrid.x, 0, horizontalCells - 1);
        int y = (int)Mathf.Clamp(pointInGrid.y, 0, verticalCells - 1);

        return x + y * horizontalCells;
    }

    private void StartHashTable()
    {
        int numLines = lines.Count;
        used = new int[cellAmount];
        initial = new int[cellAmount];
        final = new int[cellAmount];
        objectIndex = new int[numLines];
        hashTable = new Line[numLines];

        for (int i = 0; i < cellAmount; i++)
        {
            used[i] = 0;
        }
    }

    public void SpatialHashingUpdate()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            int index = HashFunction(lines[i].Start);
            objectIndex[i] = index;
            used[index]++;
        }

        int accum = 0;
        for (int i = 0; i < used.Length; i++)
        {
            initial[i] = accum;
            accum += used[i];
            final[i] = accum;
        }

        for (int i = 0; i < lines.Count; i++)
        {
            hashTable[final[objectIndex[i]] - 1] = lines[i];
            final[objectIndex[i]] -= 1;
        }
    }

    public void DebugRenderCells()
    {
        for (int x = 0; x < horizontalCells; x++)
        {
            for (int y = 0; y < verticalCells; y++)
            {
                Vector2 cellStart = new Vector2(x * cellDimension.x, y * cellDimension.y);
                Vector2 cellEnd = cellStart + cellDimension;

                Debug.DrawLine(new Vector3(cellStart.x, cellStart.y, 0), new Vector3(cellEnd.x, cellStart.y, 0), Color.red);
                Debug.DrawLine(new Vector3(cellStart.x, cellStart.y, 0), new Vector3(cellStart.x, cellEnd.y, 0), Color.red);
            }
        }
    }
}
