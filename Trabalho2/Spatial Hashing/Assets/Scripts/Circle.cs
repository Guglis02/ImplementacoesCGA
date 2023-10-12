using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Circle : MonoBehaviour
{
    public void Initialize(float radius)
    {
        GetComponent<SpriteRenderer>().size = new Vector2(radius, radius);
    }

    public void Collide(bool value)
    {
        if (value)
            GetComponent<SpriteRenderer>().color = Color.red;
        else
            GetComponent<SpriteRenderer>().color = Color.green;
    }

    //public void OnMousePosition(InputAction.CallbackContext value)
    //{
    //    var mousePos = value.ReadValue<Vector2>();
    //    var currentPos = transform.position;

    //    Debug.Log("Mouse pos: " + mousePos);
    //    Debug.Log("Current pos: " + currentPos);

    //    Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
    //    transform.position = new Vector3(mousePosWorld.x, mousePosWorld.y, 0f);
    //}
}
