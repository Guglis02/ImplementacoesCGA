using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] string interactMessage;
    public string objectName;

    public void Interact()
    {
        Debug.Log(interactMessage);
    }
}
