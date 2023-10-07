using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimate : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    private void Update()
    {
        float motion = agent.velocity.magnitude;
        animator.SetFloat("motion", motion);
    }
}
