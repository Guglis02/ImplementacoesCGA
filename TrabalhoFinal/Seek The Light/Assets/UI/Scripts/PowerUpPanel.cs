using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_timerText;
    [SerializeField]
    private Image m_radialImage;

    private float m_powerUpTime;
    private float m_timer;

    private void Start()
    {
        m_powerUpTime = Player.PowerUpDuration;
        m_timerText.text = "0";
        GameManager.Player.OnPlayerPowerUp += OnPlayerPowerUp;
    }

    private void Update()
    {
        if (m_timer <= 0)
        {
            return;
        }

        float newTime = m_timer - Time.deltaTime;
        m_timerText.text = Mathf.CeilToInt(newTime).ToString();
        m_timer = Mathf.Clamp(newTime, 0, m_powerUpTime);
        m_radialImage.fillAmount = m_timer / m_powerUpTime;
    }

    private void OnPlayerPowerUp()
    {
        m_timer = m_powerUpTime;
    }
}