using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected enum DectectionMode {Trigger, Distance, Raycast}
    [SerializeField] protected DectectionMode detectionMode;

    [SerializeField] float minDistance;

    [SerializeField] GameObject interactionPopup;
    [SerializeField] float distanceFromPivot;

    int stoppedInteractingTick;

    private void Start()
    {
       
        if(detectionMode == DectectionMode.Distance)
            StartCoroutine("CheckDistanceTick");
    }

    
    void Update()
    {
        stoppedInteractingTick ++;
  
        if(stoppedInteractingTick >= 3)
           StopDisplayingInteractionUI();
           
    }

    
   IEnumerator CheckDistanceTick()
    {
        yield return new WaitForEndOfFrame(); // this sometime initilizes before the player
        PlayerController controller = PlayerController.playerInstance;
        while(true)
        {
            if (Vector3.Distance(transform.position, controller.transform.position) < minDistance)
                BaseTriggerEvents();

            yield return new WaitForSeconds(0.05f);
        }
       
    }

    public void RayTrigger()
    {
        if(detectionMode == DectectionMode.Raycast)
            BaseTriggerEvents();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && detectionMode == DectectionMode.Trigger)
            BaseTriggerEvents();
        
    }


    void BaseTriggerEvents()
    {
        stoppedInteractingTick=0;
        DisplayInteractionUI();
        TriggerEvent();
    }

    protected void TriggerEvent()
    {
        print("Base Trigger Event!");
    }
    protected void DisplayInteractionUI()
    {
        if(interactionPopup.gameObject == null)
            return;
        interactionPopup.gameObject.SetActive(true);
        interactionPopup.transform.LookAt(PlayerController.playerInstance.transform.position);
        interactionPopup.transform.position = transform.position + (interactionPopup.transform.forward * distanceFromPivot);
    }
    
    protected void StopDisplayingInteractionUI()
    {
        print("stopped interaction");
        interactionPopup.gameObject.SetActive(false);
    }
}
