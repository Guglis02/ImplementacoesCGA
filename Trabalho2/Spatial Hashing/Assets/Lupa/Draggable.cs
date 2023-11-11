using UnityEngine;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour
{
    private Vector2 mousePos;
    public bool isDragging = false;

    public GameObject indicator;

    public float Radius => indicator.transform.localScale.x / 2;

    public void Update()
    {
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        if (Input.GetMouseButtonDown(0) && !(Vector2.Distance(mousePosWorld, transform.position) < Radius))
        {
            isDragging = true;
        }

        isDragging = false;
    }

    public void OnMousePosition(InputAction.CallbackContext value)
    {
        mousePos = value.ReadValue<Vector2>();

        if (!isDragging)
        {
            return;
        }

        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(mousePosWorld.x, mousePosWorld.y, 0f);
    }
}
