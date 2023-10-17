using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Classe responsável por controlar a câmera.
/// </summary>
public class CameraZoomController : MonoBehaviour
{
    private new Camera camera;
    private Vector2 lastMousePos;
    private Vector2 mouseDelta;

    private float initialZoom;

    private bool isPanning = false;

    private void Start()
    {
        camera = GetComponent<Camera>();
        initialZoom = camera.orthographicSize;
    }

    /// <summary>
    /// Ao rodar o scroll do mouse, a câmera dá zoom in/out na posição do mouse.
    /// </summary>
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

    /// <summary>
    /// Salva a posição do mouse, caso o botão esquerdo esteja clicado,
    /// arrasta a câmera.
    /// </summary>
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
