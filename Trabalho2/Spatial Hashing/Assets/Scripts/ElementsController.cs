using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Classe responsável por controlar os elementos da cena (circulos e linhas).
/// Lida com inserção dos mesmos e checagem de colisões.
/// </summary>
public class ElementsController : MonoBehaviour
{
    [SerializeField] private Transform planeTransform;
    [SerializeField] private UIController uiController;
    [SerializeField] private TimeTracker timeTracker;
    [SerializeField] private Circle circlePrefab;
    [SerializeField] private Line linePrefab;

    private float planeWidth;
    private float planeHeight;

    private readonly List<Line> lines = new();
    private readonly List<Circle> circles = new();

    private SpatialHash spatialHash;

    private void Start()
    {
        planeWidth = planeTransform.localScale.x;
        planeHeight = planeTransform.localScale.y;

        spatialHash = new SpatialHash((int)planeWidth, (int)planeHeight);
    }

    private void Update()
    {
        DebugUpdate();
    }

    #region Collisions
    /// <summary>
    /// Compara todos os círculos com todas as linhas para verificar se há colisão.
    /// </summary>
    public void CheckCollisionsBruteForce()
    {
        float time = Time.realtimeSinceStartup;

        foreach (Circle circle in circles)
        {
            foreach (Line line in lines)
            {
                if (UtilitaryMethods.LineCircleIntersection(line, circle))
                {
                    circle.Collide(true);
                    break;
                }
            }
        }

        Debug.Log("Tempo com força bruta: " + (Time.realtimeSinceStartup - time));
        timeTracker.SetBruteForceTime(Time.realtimeSinceStartup - time);
    }

    /// <summary>
    /// Checa colisões utilizando a tabela hash.
    /// </summary>
    public void CheckCollisionsSpatialHashing()
    {
        float time = Time.realtimeSinceStartup;

        foreach (Circle circle in circles)
        {
            spatialHash.CheckHashIntersection(circle);
        }

        Debug.Log("Tempo com hash: " + (Time.realtimeSinceStartup - time));
        timeTracker.AddSpatialHashTime(Time.realtimeSinceStartup - time);
    }

    /// <summary>
    /// Resta indicadores de colisão dos círculos.
    /// </summary>
    public void ResetCollisions()
    {
        foreach (Circle circle in circles)
        {
            circle.Collide(false);
        }
    }

    #endregion

    #region Generate Elements

    /// <summary>
    /// Gera os pontos de controle da curva.
    /// </summary>
    public void GenerateControlPoints()
    {
        List<Vector3> controlPoints = new List<Vector3>();

        int numberOfPoints = uiController.NumberOfPoints;

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float x = Random.Range(0.0f, planeWidth);
            float y = Random.Range(0.0f, planeHeight);

            Vector3 point = new Vector3(x, y, 0f);

            controlPoints.Add(point);
        }

        EvaluateLinePoints(controlPoints);

        timeTracker.Reset();
    }

    /// <summary>
    /// Avalia a curva BSpline a partir dos pontos de controle.
    /// </summary>
    private void EvaluateLinePoints(List<Vector3> controlPoints)
    {
        foreach (Line line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();

        float stepSize = 0.01f;

        for (int i = 0; i < controlPoints.Count() - 3; i++)
        {
            for (float t = 0; t <= 1; t += stepSize)
            {
                Vector3 p1 = UtilitaryMethods.BSpline3(controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t);
                Vector3 p2 = UtilitaryMethods.BSpline3(controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t + stepSize);

                Line line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                line.transform.SetParent(transform);
                line.Initialize(p1, p2);
                lines.Add(line);
            }
        }
    }
    
    /// <summary>
    /// Gera a tabela hash com as linhas geradas.
    /// </summary>
    public void GenerateSpatialHashTable()
    {
        float time = Time.realtimeSinceStartup;

        spatialHash.SetLines(lines);

        Debug.Log("Tempo para criar tabela hash: " + (Time.realtimeSinceStartup - time));
        timeTracker.SetSpacialHashingTime(Time.realtimeSinceStartup - time);
    }

    /// <summary>
    /// Gera os círculos em posicoes aleatorias.
    /// </summary>
    public void GenerateCircles()
    {
        foreach (Circle circle in circles)
        {
            Destroy(circle.gameObject);
        }
        circles.Clear();

        int numberOfCircles = uiController.NumberOfCircles;
        float circleRadius = uiController.CircleRadius;

        for (int i = 0; i < numberOfCircles; i++)
        {
            float x = Random.Range(0.0f, planeWidth);
            float y = Random.Range(0.0f, planeHeight);

            Vector3 point = new Vector3(x, y, 0f);

            Circle circle = Instantiate(circlePrefab, point, Quaternion.identity);
            circle.transform.SetParent(transform);

            circle.Initialize(circleRadius);
            circles.Add(circle);
        }
    }
    #endregion

    #region Debug

    private bool isDebugging = false;

    public void OnDebug(InputAction.CallbackContext value)
    {
        isDebugging = !isDebugging;
    }

    /// <summary>
    /// Renderiza gizmos para debugar a tabela hash.
    /// Caso o modo de depuração esteja ativado, colide em tempo real o círculo com a tabela hash.
    /// </summary>
    private void DebugUpdate()
    {
        spatialHash.DebugRenderCells();

        if (isDebugging)
        {
            foreach (Circle circle in circles)
            {
                if (circle.isDebugging)
                    spatialHash.CheckHashIntersection(circle);
            }
        }
    }

    #endregion
    }