using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text pointsText;

    private int totalPoints;

    private void Start()
    {
        totalPoints = GameManager.Instance.TotalPoints;
        pointsText.text = $"0/{totalPoints}";
        GameManager.Player.OnPointPickup += UpdatePoints;
    }

    private void UpdatePoints(int value)
    {
        pointsText.text = $"{value}/{totalPoints}";
    }
}
