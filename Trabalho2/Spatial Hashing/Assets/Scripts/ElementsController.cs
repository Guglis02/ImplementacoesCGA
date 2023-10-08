using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class ElementsController : MonoBehaviour
{
    [SerializeField] private int numberOfPoints = 200;
    [SerializeField] private float splineWidth = 0.1f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform planeTransform; 
    
    [SerializeField] private Circle circlePrefab;
    [SerializeField] private int numberOfCircles = 200;
    [SerializeField] private float circleRadius = 0.1f;

    private float planeWidth = 20f;
    private float planeHeight = 10f;

    private List<Vector3> controlPoints = new List<Vector3>();
    private List<Vector3> linePoints = new List<Vector3>();

    private List<Circle> circles = new List<Circle>();

    private void Start()
    {
        lineRenderer.startWidth = splineWidth;
        lineRenderer.endWidth = splineWidth;

        planeWidth = planeTransform.localScale.x;
        planeHeight = planeTransform.localScale.y;
    }

    public void GenerateCircles()
    {
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
        controlPoints.Clear();
        controlPoints.Add(Vector3.zero);

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            float x = Random.Range(-planeWidth * 0.5f, planeWidth * 0.5f);
            float y = Random.Range(-planeHeight * 0.5f, planeHeight * 0.5f);

            Vector3 point = new Vector3(x, y, 0f);

            controlPoints.Add(point);
        }

        EvaluateLinePoints();
    }

    private void EvaluateLinePoints()
    {
        linePoints.Clear();

        for (int i = 0; i < controlPoints.Count() - 3; i++)
        {
            for (float t = 0; t <= 1; t += 0.1f)
            {
                Vector3 p = BSpline3(controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t);
                linePoints.Add(p);
            }
        }

        RenderSpline();
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

    private void RenderSpline()
    {
        if (linePoints.Count < 2)
            return;

        lineRenderer.positionCount = linePoints.Count;

        for (int i = 0; i < linePoints.Count; i++)
        {
            lineRenderer.SetPosition(i, linePoints[i]);
        }
    }
}