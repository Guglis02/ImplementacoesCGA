using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CameraZoomController : MonoBehaviour
{
    private Camera camera;
    private Vector2 lastMousePos;
    private Vector2 mouseDelta;

    private float initialZoom;

    private void Start()
    {
        camera = GetComponent<Camera>();    
        initialZoom = camera.orthographicSize;
    }

    public void OnMouseWheel(InputAction.CallbackContext value)
    {
        float scroll = Mathf.Clamp(value.ReadValue<float>(), -1, 1);
        camera.orthographicSize -= scroll;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 1f, initialZoom);
    }

    public void OnMousePosition(InputAction.CallbackContext value)
    {
        Vector2 mousePos = value.ReadValue<Vector2>();
        mouseDelta = mousePos - lastMousePos;
        lastMousePos = mousePos;
    }
}
