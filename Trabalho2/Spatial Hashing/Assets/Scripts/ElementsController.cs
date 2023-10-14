using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    #region Collisions
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

                circle.Collide(false);
            }
        }

        Debug.Log("Tempo com força bruta: " + (Time.realtimeSinceStartup - time));
        timeTracker.SetBruteForceTime(Time.realtimeSinceStartup - time);
    }

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

    public void ResetCollisions()
    {
        foreach (Circle circle in circles)
        {
            circle.Collide(false);
        }

        timeTracker.Reset();
    }

    #endregion

    #region Generate Elements
    public void GenerateSpatialHashTable()
    {
        float time = Time.realtimeSinceStartup;
        spatialHash.SetLines(lines);
        Debug.Log("Tempo para criar tabela hash: " + (Time.realtimeSinceStartup - time));
        timeTracker.SetSpacialHashingTime(Time.realtimeSinceStartup - time);
    }

    public void GenerateCircles()
    {
        foreach (Circle circle in circles)
        {
            Destroy(circle.gameObject);
        }
        circles.Clear();

        int numberOfCircles = uiController.NumberOfCircles;

        for (int i = 0; i < numberOfCircles; i++)
        {
            float x = Random.Range(-planeWidth * 0.5f, planeWidth * 0.5f);
            float y = Random.Range(-planeHeight * 0.5f, planeHeight * 0.5f);

            Vector3 point = new Vector3(x, y, 0f);

            Circle circle = Instantiate(circlePrefab, point, Quaternion.identity);
            circle.transform.SetParent(transform);

            float circleRadius = uiController.CircleRadius;
            circle.Initialize(circleRadius);
            circles.Add(circle);
        }
    }

    public void GenerateControlPoints()
    {
        List<Vector3> controlPoints = new List<Vector3>();

        int numberOfPoints = uiController.NumberOfPoints;

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float x = Random.Range(-planeWidth * 0.5f, planeWidth * 0.5f);
            float y = Random.Range(-planeHeight * 0.5f, planeHeight * 0.5f);

            Vector3 point = new Vector3(x, y, 0f);

            controlPoints.Add(point);
        }

        EvaluateLinePoints(controlPoints);
    }

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
                List<Vector3> segmentPoints = new List<Vector3>();

                Vector3 p1 = UtilitaryMethods.BSpline3(controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t);
                Vector3 p2 = UtilitaryMethods.BSpline3(controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t + stepSize);
                segmentPoints.Add(p1);
                segmentPoints.Add(p2);

                Line line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                line.transform.SetParent(transform);
                line.Initialize(segmentPoints);
                lines.Add(line);
            }
        }
    }
    #endregion
}