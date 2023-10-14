using System;
using System.Collections.Generic;
using UnityEngine;

public class SpatialHash
{
    private Vector2 cellDimension;
    private int horizontalCells = 5;
    private int verticalCells = 5;

    private List<Line> lines = new List<Line>();
    private List<int> used = new List<int>();
    private List<int> initial = new List<int>();
    private List<int> final = new List<int>();
    private List<int> objectIndex = new List<int>();
    private List<Line> hashTable = new List<Line>();

    public SpatialHash(int planeWidth, int planeHeight)
    {
        this.cellDimension = new Vector2(planeWidth / horizontalCells, planeHeight / verticalCells);
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

        if (cellIndex >= 0 && cellIndex < hashTable.Count)
        {
            for (int i = initial[cellIndex]; i < final[cellIndex]; i++)
            {
                Line line = hashTable[i];

                if (UtilitaryMethods.LineCircleIntersection(line, circle))
                {
                    circle.Collide(true);
                }
            }
        }

        circle.Collide(false);
    }

    private int HashFunction(Vector2 point)
    {
        Vector2 pointInGrid = point / cellDimension;

        return (int)pointInGrid.x + (int)pointInGrid.y * horizontalCells;
    }
    private void StartHashTable()
    {
        throw new NotImplementedException();
    }

    public void SpatialHashingUpdate()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            int index = HashFunction(lines[i].Start);
            objectIndex[i] = index;
            if (!used.ContainsKey(index))
            {
                used[index] = 1;
            }
            else
            {
                used[index]++;
            }
        }

        int accum = 0;
        for (int i = 0; i < used.Count; i++)
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
}
