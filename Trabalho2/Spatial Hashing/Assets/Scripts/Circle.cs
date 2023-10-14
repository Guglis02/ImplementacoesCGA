using UnityEngine;

public class Circle : MonoBehaviour
{
    private float m_radius = 1;

    public float Radius => m_radius;

    public void Initialize(float radius)
    {
        this.m_radius = radius;
        float scale = radius * 2;
        transform.localScale = new Vector3(scale, scale, 1);
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
