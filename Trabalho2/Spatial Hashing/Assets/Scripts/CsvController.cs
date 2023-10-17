using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Classe responsável por gerenciar um arquivo csv com os dados dos testes.
/// </summary>
public class CsvController : MonoBehaviour
{
    [SerializeField] private Transform planeTransform;
    [SerializeField] private UIController uiController;
    [SerializeField] private TimeTracker timeTracker;

    private string filePath;

    private void Start()
    {
        filePath = "Assets/Data/data.csv";

        if (!File.Exists(filePath))
        {
            string headers = "Plane Width,Plane Height,Number of Circles,Circle Radius,Number of Control Points,Brute Force Time,Spatial Hashing Time";
            File.WriteAllText(filePath, headers + Environment.NewLine);
        }
    }

    public void SaveValues()
    {
        List<string> lines = new List<string>(File.ReadAllLines(filePath));

        string[] data = { 
            planeTransform.localScale.x.ToString(),
            planeTransform.localScale.y.ToString(),
            uiController.NumberOfCircles.ToString(),
            uiController.CircleRadius.ToString(),
            uiController.NumberOfPoints.ToString(),
            timeTracker.BruteForceTime.ToString(),
            timeTracker.SpacialHashingTime.ToString()
        };

        lines.Add(string.Join(",", data));

        File.WriteAllLines(filePath, lines);

        Debug.Log("Dados salvos com sucesso!");
    }
}

