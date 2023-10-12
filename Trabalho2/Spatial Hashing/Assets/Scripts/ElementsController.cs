using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class ElementsController : MonoBehaviour
{
    [SerializeField] private int numberOfPoints = 200;
    [SerializeField] private float splineWidth = 0.1f;
    [SerializeField] private Transform planeTransform; 
    
    [SerializeField] private Circle circlePrefab;
    [SerializeField] private Line linePrefab;

    [SerializeField] private int numberOfCircles = 200;
    [SerializeField] private float circleRadius = 0.1f;

    private float planeWidth = 20f;
    private float planeHeight = 10f;

    private List<Line> lines = new List<Line>();
    private List<Circle> circles = new List<Circle>();

    private void Start()
    {
        planeWidth = planeTransform.localScale.x;
        planeHeight = planeTransform.localScale.y;
    }

    public void CheckCollisions()
    {
        float time = Time.realtimeSinceStartup;

        foreach (Circle circle in circles)
        {
            foreach (Line line in lines)
            {
                if (LineCircleIntersection(line.points.First(), line.points.Last(), circle.transform.position, circleRadius))
                {
                    circle.Collide(true);
                    break;
                }

                circle.Collide(false);
            }
        }

        Debug.Log("Tempo com força bruta: " + (Time.realtimeSinceStartup - time));
    }

    bool LineCircleIntersection(Vector2 lineStart, Vector2 lineEnd, Vector2 circleCenter, float circleRadius)
    {
        // Calculate the direction vector of the line segment
        Vector2 lineDirection = lineEnd - lineStart;

        // Calculate the vector from the start of the line to the circle center
        Vector2 lineToCircle = circleCenter - lineStart;

        // Calculate the length of the line segment
        float lineLength = lineDirection.magnitude;

        // Normalize the line direction vector
        lineDirection /= lineLength;

        // Calculate the projection of lineToCircle onto the line direction
        float t = Vector2.Dot(lineToCircle, lineDirection);

        // Calculate the closest point on the line to the circle center
        Vector2 closestPoint;
        if (t < 0) // Closest point is lineStart
        {
            closestPoint = lineStart;
        }
        else if (t > lineLength) // Closest point is lineEnd
        {
            closestPoint = lineEnd;
        }
        else // Closest point is between lineStart and lineEnd
        {
            closestPoint = lineStart + lineDirection * t;
        }

        // Check if the distance between the closest point and the circle center is less than the circle radius
        float distanceToCenter = (circleCenter - closestPoint).magnitude;

        return distanceToCenter <= circleRadius;
    }

    public void GenerateCircles()
    {
        foreach (Circle circle in circles)
        {
            Destroy(circle.gameObject);
        }
        circles.Clear();

        for (int i = 0; i < numberOfCircles; i++)
        {
            float x = Random.Range(-planeWidth * 0.5f, planeWidth * 0.5f);
            float y = Random.Range(-planeHeight * 0.5f, planeHeight * 0.5f);

            Vector3 point = new Vector3(x, y, 0f);

            Circle circle = Instantiate(circlePrefab, point, Quaternion.identity);
            circle.Initialize(circleRadius);
            circles.Add(circle);
        }
    }

    public void GenerateControlPoints()
    {
        List<Vector3> controlPoints = new List<Vector3>();
        controlPoints.Add(Vector3.zero);

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

                Vector3 p1 = BSpline3(controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t);
                Vector3 p2 = BSpline3(controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t + stepSize);
                segmentPoints.Add(p1);
                segmentPoints.Add(p2);

                Line line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                line.InitializeLine(segmentPoints);
                lines.Add(line);
            }
        }
    }

    private Vector3 BSpline3(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
    {
        float asixth = 1.0f / 6.0f;
        float tcube = t * t * t;
        float tsquare = t * t;

        return p1 * asixth * Mathf.Pow((1 - t), 3)
            + p2 * asixth * (3 * tcube - 6 * tsquare + 4)
            + p3 * asixth * (-3 * tcube + 3 * tsquare + 3 * t + 1)
            + p4 * asixth * tcube;
    }  
}