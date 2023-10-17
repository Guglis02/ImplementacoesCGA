using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe que representa um trecho da curva BSpline.
/// </summary>
public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private Vector3 m_StartPoint;
    private Vector3 m_EndPoint;

    public Vector2 Start => m_StartPoint;
    public Vector2 End => m_EndPoint;

    public void Initialize(Vector3 startPoint, Vector3 endPoint)
    {
        m_StartPoint = startPoint;
        m_EndPoint = endPoint;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new[] { startPoint, endPoint });
    }
}