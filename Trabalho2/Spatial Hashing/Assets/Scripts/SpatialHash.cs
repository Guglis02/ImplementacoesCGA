using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe responsável por implementar a Spatial Hash Table, descrita no artigo
/// do professor.
/// </summary>
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
        cellDimension = new Vector2(planeWidth / horizontalCells, planeHeight / verticalCells);
        cellAmount = horizontalCells * verticalCells;
    }

    /// <summary>
    /// Define as linhas que serão salvas na tabela hash.
    /// </summary>
    public void SetLines(List<Line> lines)
    {
        this.lines = lines;
        StartHashTable();
        SpatialHashingUpdate();
    }

    /// <summary>
    /// Calcula em qual célula da tabela hash o círculo está e verifica se há interseção
    /// com alguma linha daquela célula.
    /// </summary>
    public void CheckHashIntersection(Circle circle)
    {
        List<int> rangedCells = CalculateRangedCells(circle);

        foreach (int cellIndex in rangedCells)
        {
            if (used[cellIndex] == 0)
            {
                continue;
            }

            for (int i = initial[cellIndex]; i < initial[cellIndex] + used[cellIndex]; i++)
            {
                Line line = hashTable[i];

                if (UtilitaryMethods.LineCircleIntersection(line, circle))
                {
                    circle.Collide(true);
                    return;
                }

                circle.Collide(false);
            }
        }
    }

    /// <summary>
    /// Procura em quais células o círculo está.
    /// </summary>
    private List<int> CalculateRangedCells(Circle circle)
    {
        List<int> rangedCells = new List<int>();
        Vector2 circleCenter = circle.transform.position;
        float circleRadius = circle.Radius;

        int centerIndex = HashFunction(circleCenter);
        rangedCells.Add(centerIndex);

        for (float i = 0; i < 360; i += 45)
        {
            Vector2 circlePoint = new Vector2(circleCenter.x + circleRadius * Mathf.Cos(i), circleCenter.y + circleRadius * Mathf.Sin(i));
            if (HashFunction(circlePoint) != centerIndex)
                rangedCells.Add(HashFunction(circlePoint));
        }

        return rangedCells;
    }
    
    /// <summary>
    /// Calcula em qual célula da tabela hash o ponto está.
    /// </summary>
    private int HashFunction(Vector2 point)
    {
        Vector2 pointInGrid = point / cellDimension;

        int x = (int)Mathf.Clamp(pointInGrid.x, 0, horizontalCells - 1);
        int y = (int)Mathf.Clamp(pointInGrid.y, 0, verticalCells - 1);

        return x + y * horizontalCells;
    }

    /// <summary>
    /// Inicializa estruturas de dados necessárias para a tabela hash.
    /// </summary>
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

    /// <summary>
    /// Atualiza pivos e hash table, como os elementos salvos na tabela não se movem,
    /// este método não precisa ser chamado todo frame, mas sim apenas quando os 
    /// elementos são recriados.
    /// </summary>
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

    /// <summary>
    /// Função de debug que desenha as células da tabela hash.
    /// </summary>
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
