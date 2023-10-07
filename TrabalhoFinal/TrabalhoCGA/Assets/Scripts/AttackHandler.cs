using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private float attackRange = 1f;

    private InteractableObject target;

    public void Attack(InteractableObject target)
    {
        this.target = target;
        ProcessAttack(target);
    }

    private void Update()
    {
        if (target != null)
        {
            ProcessAttack(target);
        }
    }

    private void ProcessAttack(InteractableObject target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance < attackRange)
        {
            characterMovement.Stop();
            animator.SetTrigger("Attack");
            this.target = null;
        }
        else
        {
            characterMovement.SetDestination(target.transform.position);
        }
    }
}
