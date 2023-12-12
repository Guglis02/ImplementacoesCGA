using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Lantern : MonoBehaviour
{
    [SerializeField]
    private GameObject volumetricLight;

    [SerializeField]
    private Light pointLight;

    [SerializeField]
    private float maxLightIntensity;

    [SerializeField]
    private float minLightIntensity;

    [SerializeField]
    private float decayRate;

    private void Start()
    {
        GameManager.Player.OnPlayerPowerUp += OnPlayerPowerUp;
        GameManager.Player.OnPlayerPowerDown += OnPlayerPowerDown;
        GameManager.Player.OnPointPickup += OnPlayerPickup;
    }

    private void OnPlayerPickup(int _)
    {
        pointLight.intensity = maxLightIntensity;
    }

    private void OnPlayerPowerUp()
    {
        volumetricLight.SetActive(true);
    }

    private void OnPlayerPowerDown()
    {
        volumetricLight.SetActive(true);
    }

    private void Update()
    {
        if (pointLight.intensity >= minLightIntensity) 
        {
            pointLight.intensity -= decayRate;
        }
    }
}
