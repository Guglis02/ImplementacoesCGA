using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInput : MonoBehaviour
{
    [SerializeField] InteractInput interactInput;
    [SerializeField] AttackHandler attackHandler;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (interactInput.HoveringOverObject != null)
            {
                attackHandler.Attack(interactInput.HoveringOverObject);
            }
        }
    }
}
