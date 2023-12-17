using System;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    public int MaxHealth = 3;

    public const float PowerUpDuration = 30f;
    public Vector3 Forward => transform.forward;

    public Action<int> OnPlayerHit;
    public Action<int> OnPointPickup;
    public Action OnPlayerPowerUp;
    public Action OnPlayerPowerDown;
    public Action OnPlayerDeath;
    public Action OnPlayerGotAllPoints;

    private CharacterController m_characterController;

    private int m_health = 3;
    private int m_points = 0;

    private float m_powerUpTimer = 0f;

    private bool IsPoweredUp => m_powerUpTimer > 0;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            HandleEnemyCollision(enemy);
        }
        else if (other.gameObject.CompareTag("AttackCollider"))
        {
            HandleEnemyCollision(null);
        }
        else if (other.gameObject.TryGetComponent(out Pickup pickup))
        {
            if (pickup.Effect == Pickup.PickupEffect.Point)
            {
                m_points++;
                OnPointPickup?.Invoke(m_points);
            }
            else if (pickup.Effect == Pickup.PickupEffect.PowerUp)
            {
                m_powerUpTimer = PowerUpDuration;
                OnPlayerPowerUp?.Invoke();
            }
            Destroy(pickup.gameObject);
        }
    }

    private void HandleEnemyCollision(Enemy enemy)
    {
        if (IsPoweredUp)
        {
            enemy?.Die();
            return;
        }

        TakeDamage();
    }

    public void TakeDamage()
    {
        m_health--;
        OnPlayerHit?.Invoke(m_health);
        if (m_health <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }

    private void Update()
    {
        if (m_points == GameManager.Instance.TotalPoints)
        {
            OnPlayerGotAllPoints?.Invoke();
        }

        if (IsPoweredUp)
        {
            UpdatePowerUp();
        }
    }

    private void UpdatePowerUp()
    {
        m_powerUpTimer -= Time.deltaTime;
        if (m_powerUpTimer <= 0f)
        {
            OnPlayerPowerDown?.Invoke();
        }
    }

    public void SetPosition(Vector3 position)
    {
        m_characterController.SetPosition(position);
    }
}
