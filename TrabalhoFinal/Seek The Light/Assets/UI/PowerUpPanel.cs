using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image radialImage;

    private float powerUpTime;
    private float timer;

    private void Start()
    {
        powerUpTime = Player.PowerUpDuration;
        timerText.text = "0";
        GameManager.Player.OnPlayerPowerUp += OnPlayerPowerUp;
    }

    private void Update()
    {
        if (timer <= 0)
        {
            return;
        }

        float newTime = timer - Time.deltaTime;
        timerText.text = Mathf.CeilToInt(newTime).ToString();
        timer = Mathf.Clamp(newTime, 0, powerUpTime);
        radialImage.fillAmount = timer / powerUpTime;
    }

    private void OnPlayerPowerUp()
    {
        timer = powerUpTime;
    }
}