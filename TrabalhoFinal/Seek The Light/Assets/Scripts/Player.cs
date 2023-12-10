using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private CharacterController m_CharacterController;

    public bool IsPoweredUp = false;

    public int MaxHealth = 3;
    
    private int health = 3;
    private int points = 0;

    public Vector3 Forward => transform.forward;

    public Action<int> OnPlayerHit;
    public Action<int> OnPointPickup;
    public Action OnPlayerPowerUp;
    public Action OnPlayerPowerDown;
    public Action OnPlayerDeath;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Enemy>(out Enemy enemy)
            || other.gameObject.CompareTag("AttackCollider"))
        {
            if (IsPoweredUp)
            {
                enemy.OnEaten();
            }
            else
            {
                health--;
                OnPlayerHit?.Invoke(health);
                if (health <= 0)
                {
                    OnPlayerDeath?.Invoke();
                }
            }
        }
        else if (other.gameObject.TryGetComponent<Pickup>(out Pickup pickup))
        {
            if (pickup.pickupType == Pickup.PickupType.Point)
            {
                points++;
                OnPointPickup?.Invoke(points);
                Destroy(pickup.gameObject);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Teste();
        }
    }

    public void Teste()
    {
        Debug.Log("Teste");
        OnPlayerHit?.Invoke(health);
    }

    public void SetPosition(Vector3 position)
    {
        m_CharacterController.SetPosition(position);
    }
}
