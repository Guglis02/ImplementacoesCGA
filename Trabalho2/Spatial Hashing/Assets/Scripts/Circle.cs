using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public void Initialize(float radius)
    {
        GetComponent<SpriteRenderer>().size = new Vector2(radius, radius);
    }

    public void Collide()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
    }
}
