using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController m_CharacterController;

    public bool IsPoweredUp = false;

    public int Health = 3;

    public Vector3 Forward => transform.forward;

    public Action OnPlayerHit;
    public Action OnPlayerPowerUp;

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
                Health--;
                OnPlayerHit?.Invoke();
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        bool prevCharacterControlerEnabled = m_CharacterController.enabled;

        m_CharacterController.enabled = false;
        transform.position = position;
        m_CharacterController.enabled = prevCharacterControlerEnabled;
    }
}
