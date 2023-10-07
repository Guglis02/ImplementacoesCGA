using TMPro;
using UnityEngine;

public class InteractInput : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textOnScreen;

    private InteractableObject m_hoveringOverObject;

    public InteractableObject HoveringOverObject => m_hoveringOverObject;

    private void Update()
    {
        CheckForInteractableObject();

        if (Input.GetMouseButtonDown(0))
        {
            HoveringOverObject?.Interact();
        }
    }

    private void CheckForInteractableObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            InteractableObject interactableObject = hit.transform.GetComponent<InteractableObject>();
            if (interactableObject != null)
            {
                m_hoveringOverObject = interactableObject;
                textOnScreen.text = interactableObject.objectName;
            }
            else
            {
                m_hoveringOverObject = null;
                textOnScreen.text = "";
            }
        }
    }
}
