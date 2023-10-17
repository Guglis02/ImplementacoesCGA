using UnityEngine;

/// <summary>
/// Classe de métodos utilitários.
/// </summary>
static class UtilitaryMethods
{
    public static Vector3 BSpline3(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
    {
        float asixth = 1.0f / 6.0f;
        float tcube = t * t * t;
        float tsquare = t * t;

        return p1 * asixth * Mathf.Pow((1 - t), 3)
            + p2 * asixth * (3 * tcube - 6 * tsquare + 4)
            + p3 * asixth * (-3 * tcube + 3 * tsquare + 3 * t + 1)
            + p4 * asixth * tcube;
    }

    public static bool LineCircleIntersection(Line line, Circle circle)
    {
        Vector2 lineStart = line.Start;
        Vector2 lineEnd = line.End;
        Vector2 circleCenter = circle.transform.position;
        float circleRadius = circle.Radius;

        Vector2 lineDirection = lineEnd - lineStart;
        Vector2 lineToCircle = circleCenter - lineStart;
        float lineLength = lineDirection.magnitude;

        lineDirection.Normalize();

        float t = Vector2.Dot(lineToCircle, lineDirection);

        Vector2 closestPoint;
        if (t < 0)
        {
            closestPoint = lineStart;
        }
        else if (t > lineLength)
        {
            closestPoint = lineEnd;
        }
        else
        {
            closestPoint = lineStart + lineDirection * t;
        }

        float distanceToCenter = (circleCenter - closestPoint).magnitude;

        return distanceToCenter <= circleRadius;
    }
}