using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    private HealthIndicator HealthIndicatorPrefab;
    [SerializeField]
    private RectTransform HpPanel;

    private List<HealthIndicator> healthIndicators = new List<HealthIndicator>();

    private int maxHealth;

    private void Start()
    {
        GameManager.Player.OnPlayerHit += UpdateHealth;
        maxHealth = GameManager.Player.MaxHealth;
        
        for (int i = 0; i < maxHealth; i++)
        {
            HealthIndicator healthIndicator = Instantiate(HealthIndicatorPrefab, HpPanel);
            healthIndicators.Add(healthIndicator);
        }
    }

    private void UpdateHealth(int value)
    {
        for (int i = 0; i < maxHealth; i++)
        {
            if (i < value)
            {
                healthIndicators[i].lampOnImage.SetActive(true);
            }
            else
            {
                healthIndicators[i].lampOnImage.SetActive(false);
            }
        }       
    }
}
