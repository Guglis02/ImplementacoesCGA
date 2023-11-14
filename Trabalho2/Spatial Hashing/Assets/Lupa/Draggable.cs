using UnityEngine;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour
{
    private Vector2 mousePos;
    private bool isDragging = false;

    [SerializeField] private GameObject indicator;

    public float Radius => indicator.transform.localScale.x / 2;

    public void OnMouseClick(InputAction.CallbackContext value)
    {
        Vector2 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        if (value.ReadValueAsButton() && Vector2.Distance(mousePosWorld, indicator.transform.position) < Radius)
        {
            isDragging = true;
            return;
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
