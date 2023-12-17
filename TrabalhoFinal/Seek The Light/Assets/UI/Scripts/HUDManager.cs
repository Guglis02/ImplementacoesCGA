using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    private HealthIndicator m_healthIndicatorPrefab;
    [SerializeField]
    private RectTransform m_hpPanel;

    private readonly List<HealthIndicator> m_healthIndicators = new();

    private int m_maxHealth;

    private void Start()
    {
        GameManager.Player.OnPlayerHit += UpdateHealth;
        m_maxHealth = GameManager.Player.MaxHealth;

        for (int i = 0; i < m_maxHealth; i++)
        {
            HealthIndicator healthIndicator = Instantiate(m_healthIndicatorPrefab, m_hpPanel);
            m_healthIndicators.Add(healthIndicator);
        }
    }

    private void UpdateHealth(int value)
    {
        for (int i = 0; i < m_maxHealth; i++)
        {
            m_healthIndicators[i].LampOnImage.SetActive(i < value);
        }
    }
}
