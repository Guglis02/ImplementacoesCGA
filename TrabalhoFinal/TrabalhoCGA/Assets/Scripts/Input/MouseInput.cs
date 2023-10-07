using UnityEngine;

public class MouseInput : MonoBehaviour
{
    private Vector3 m_mouseInputPosition;

    public Vector3 MouseInputPosition => m_mouseInputPosition; 
    
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue))
        {
            m_mouseInputPosition = hit.point;
        }
    }
}
