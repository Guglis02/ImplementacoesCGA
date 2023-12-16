using System;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    private CharacterController m_CharacterController;

    public int MaxHealth = 3;

    private int health = 3;
    private int points = 0;

    public const float PowerUpDuration = 30f;
    private float powerUpTimer = 0f;

    public Vector3 Forward => transform.forward;

    public Action<int> OnPlayerHit;
    public Action<int> OnPointPickup;
    public Action OnPlayerPowerUp;
    public Action OnPlayerPowerDown;
    public Action OnPlayerDeath;
    public Action OnPlayerGotAllPoints;

    private bool IsPoweredUp => powerUpTimer > 0;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void HandleEnemyCollision(Enemy enemy)
    {
        if (IsPoweredUp)
        {
            enemy.Die();
            return;
        }

        TakeDamage();
    }

    private void HandleEnemyAttack()
    {
        if (IsPoweredUp)
        {
            return;
        }

        TakeDamage();
    }

    public void TakeDamage()
    {
        health--;
        OnPlayerHit?.Invoke(health);
        if (health <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            HandleEnemyCollision(enemy);
        }
        else if (other.gameObject.CompareTag("AttackCollider"))
        {
            HandleEnemyAttack();
        }
        else if (other.gameObject.TryGetComponent(out Pickup pickup))
        {
            if (pickup.pickupType == Pickup.PickupType.Point)
            {
                points++;
                OnPointPickup?.Invoke(points);
            }
            else if (pickup.pickupType == Pickup.PickupType.PowerUp)
            {
                powerUpTimer = PowerUpDuration;
                OnPlayerPowerUp?.Invoke();
            }
            Destroy(pickup.gameObject);
        }
    }

    private void Update()
    {
        //if (points == GameManager.Instance.TotalPoints)
        //{
        //    OnPlayerGotAllPoints?.Invoke();
        //}

        if (IsPoweredUp)
        {
            UpdatePowerUp();
        }
    }

    private void UpdatePowerUp()
    {
        powerUpTimer -= Time.deltaTime;
        if (powerUpTimer <= 0f)
        {
            OnPlayerPowerDown?.Invoke();
        }
    }

    public void SetPosition(Vector3 position)
    {
        m_CharacterController.SetPosition(position);
    }
}
