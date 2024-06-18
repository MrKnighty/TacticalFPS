using UnityEngine;

public class PlayerInteractionRaycaster : MonoBehaviour
{
    
    [SerializeField] float interactionDistance;
    void Update()
    {
        if (Physics.Raycast(transform.position, Camera.main.transform.forward, out RaycastHit hit, interactionDistance))
        {
            if (hit.transform.GetComponent<Interactable>())
                hit.transform.GetComponent<Interactable>().RayTrigger();
        }
    }
}
