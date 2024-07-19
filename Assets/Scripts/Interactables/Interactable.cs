using UnityEngine;
using System.Collections;


public class Interactable : MonoBehaviour
{
    [SerializeField] protected enum DectectionMode {Trigger, Distance, Raycast}
    [SerializeField] protected DectectionMode detectionMode;

    [SerializeField] float minDistance;
    
    [SerializeField] GameObject interactionPopup;
    [SerializeField] float distanceFromPivot;
    [SerializeField] bool lookAtPlayer;
    bool interactionUiActive;

    int stoppedInteractingTick;
    [SerializeField] bool askForInteract;

    protected virtual void Start()
    {
        

        if(detectionMode == DectectionMode.Distance)
            StartCoroutine("CheckDistanceTick");
    }

    
    void Update()
    {
        stoppedInteractingTick ++;
        if(!interactionUiActive)
             return;
             
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

            yield return null;
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

    protected virtual void PreInteract()
    {
        return;
    }


    void BaseTriggerEvents()
    {
        stoppedInteractingTick = 0;
        PreInteract();
        if (askForInteract)
        {
         
            DisplayInteractionUI();

            if (!Input.GetKeyDown(KeyCode.F))
                return;

        }

        TriggerEvent();
        
    }

    protected virtual void TriggerEvent()
    {
        print("Base Trigger Event!");
    }
    protected void DisplayInteractionUI()
    {
        if(interactionPopup.gameObject == null)
            return;
            
        interactionUiActive = true;
        interactionPopup.gameObject.SetActive(true);
        if (!lookAtPlayer)
            return;
        interactionPopup.transform.LookAt(PlayerController.playerInstance.transform.position);
        interactionPopup.transform.position = transform.position + (interactionPopup.transform.forward * distanceFromPivot);
    }
    
    protected void StopDisplayingInteractionUI()
    {
        interactionUiActive = false;
        interactionPopup.gameObject.SetActive(false);
    }
}
