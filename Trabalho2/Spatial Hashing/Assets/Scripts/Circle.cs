using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Classe que representa um círculo que será mostrado na tela.
/// </summary>
public class Circle : MonoBehaviour
{
    private float m_radius = 1;

    public float Radius => m_radius;

    public void Initialize(float radius)
    {
        m_radius = radius;
        float scale = radius * 2;
        transform.localScale = new Vector3(scale, scale, 1);
    }

    /// <summary>
    /// Modifica a cor do circulo caso haja colisao.
    /// </summary>
    public void Collide(bool value)
    {
        GetComponent<SpriteRenderer>().color = value ? Color.red : Color.green;
    }

    /// <summary>
    /// Métodos e atributos para depuracao do programa.
    /// Permitem movimentar um círculo apertando a tecla D sobre ele.
    /// </summary>
    #region Debug

    private Vector2 mousePos;
    public bool isDebugging = false;

    public void OnMousePosition(InputAction.CallbackContext value)
    {
        mousePos = value.ReadValue<Vector2>();

        if (!isDebugging)
        {
            return;
        }

        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(mousePosWorld.x, mousePosWorld.y, 0f);
    }

    public void OnDebug(InputAction.CallbackContext value)
    {
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);

        if (!(Vector2.Distance(mousePosWorld, transform.position) < Radius) || isDebugging)
        {
            isDebugging = false;
            return;
        }

        isDebugging = true;
    }
    #endregion
}
