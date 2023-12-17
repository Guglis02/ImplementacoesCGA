using UnityEngine;

public class Lantern : MonoBehaviour
{
    [SerializeField]
    private GameObject m_volumetricLight;
    [SerializeField]
    private Light m_pointLight;
    [SerializeField]
    private float m_maxLightIntensity;
    [SerializeField]
    private float m_minLightIntensity;
    [SerializeField]
    private float m_decayRate;

    private void Start()
    {
        GameManager.Player.OnPlayerPowerUp += OnPlayerPowerUp;
        GameManager.Player.OnPlayerPowerDown += OnPlayerPowerDown;
        GameManager.Player.OnPointPickup += OnPlayerPickup;
    }

    private void OnPlayerPickup(int _)
    {
        m_pointLight.intensity = m_maxLightIntensity;
    }

    private void OnPlayerPowerUp()
    {
        m_volumetricLight.SetActive(true);
    }

    private void OnPlayerPowerDown()
    {
        m_volumetricLight.SetActive(false);
    }

    private void Update()
    {
        if (m_pointLight.intensity >= m_minLightIntensity)
        {
            m_pointLight.intensity -= m_decayRate * Time.deltaTime;
        }
    }
}
