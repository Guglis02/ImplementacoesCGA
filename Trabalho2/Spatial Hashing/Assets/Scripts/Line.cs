using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector3> points;

    public Vector2 Start => points[0];
    public Vector2 End => points[1];

    public void Initialize(List<Vector3> linePoints)
    {
        points = linePoints;
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}