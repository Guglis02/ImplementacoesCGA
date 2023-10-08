using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
using System.Buffers.Text;
using Unity.VisualScripting;
using UnityEditor.Presets;

public class CameraZoomController : MonoBehaviour
{
    private Camera camera;
    private Vector2 lastMousePos;
    private Vector2 mouseDelta;

    private float initialZoom;

    private bool isPanning = false;

    private void Start()
    {
        camera = GetComponent<Camera>();    
        initialZoom = camera.orthographicSize;
    }

    public void OnMouseWheel(InputAction.CallbackContext value)
    {
        float scroll = Mathf.Clamp(value.ReadValue<float>(), -1, 1);
        Vector3 mousePos = camera.ScreenToWorldPoint(lastMousePos);
        camera.orthographicSize -= scroll;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 1f, initialZoom); 
        Vector3 newMousePos = camera.ScreenToWorldPoint(lastMousePos);
        Vector3 deltaPos = newMousePos - mousePos;
        camera.transform.position -= deltaPos;
    }

    public void OnMousePosition(InputAction.CallbackContext value)
    {
        Vector2 mousePos = value.ReadValue<Vector2>();
        mouseDelta = mousePos - lastMousePos;
        lastMousePos = mousePos;

        if (isPanning)
        {
            Vector3 delta = new Vector3(-mouseDelta.x, -mouseDelta.y, 0) * camera.orthographicSize * 0.01f;
            camera.transform.Translate(delta);
        }
    }

    public void OnMouseClick(InputAction.CallbackContext value)
    {
        isPanning = value.ReadValueAsButton();
    }
}
