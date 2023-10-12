using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField numberOfPointsInput;
    [SerializeField] private TMP_InputField numberOfCirclesInput;
    [SerializeField] private TMP_InputField circleRadiusInput;

    [SerializeField] private const int numberOfPoints = 200;
    [SerializeField] private const int numberOfCircles = 200;
    [SerializeField] private const float circleRadius = 1f;

    public int NumberOfPoints => int.Parse(numberOfPointsInput.text);
    public int NumberOfCircles => int.Parse(numberOfCirclesInput.text);
    public float CircleRadius => float.Parse(circleRadiusInput.text);

    private void Start()
    {
        numberOfPointsInput.text = numberOfPoints.ToString();
        numberOfCirclesInput.text = numberOfCircles.ToString();
        circleRadiusInput.text = circleRadius.ToString();
    }
}
