using TMPro;
using UnityEngine;

public class PointsPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_pointsText;

    private int m_totalPoints;

    private void Start()
    {
        m_totalPoints = GameManager.Instance.TotalPoints;
        m_pointsText.text = $"0/{m_totalPoints}";
        GameManager.Player.OnPointPickup += UpdatePoints;
    }

    private void UpdatePoints(int value)
    {
        m_pointsText.text = $"{value}/{m_totalPoints}";
    }
}
