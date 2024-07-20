using UnityEngine;
using System.Collections;


public class Interactable : MonoBehaviour
{
    [SerializeField] protected enum DectectionMode { Trigger, Distance, Raycast }
    [SerializeField] protected DectectionMode detectionMode;

    [SerializeField] float minDistance;


    int stoppedInteractingTick;
    [SerializeField] bool askForInteract;

    protected virtual void Start()
    {


        if (detectionMode == DectectionMode.Distance)
            StartCoroutine("CheckDistanceTick");
    }



    IEnumerator CheckDistanceTick()
    {
        yield return new WaitForEndOfFrame(); // this sometime initilizes before the player
        PlayerController controller = PlayerController.playerInstance;
        while (true)
        {

            if (Vector3.Distance(transform.position, controller.transform.position) < minDistance)
                BaseTriggerEvents();

            yield return null;
        }

    }

    public void RayTrigger()
    {
        if (detectionMode == DectectionMode.Raycast)
            BaseTriggerEvents();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player" && detectionMode == DectectionMode.Trigger)
        {

            BaseTriggerEvents();
        }


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



            if (!Input.GetKeyDown(KeyCode.F))
                return;


        }

        TriggerEvent();

    }

    protected virtual void TriggerEvent()
    {
        print("Base Trigger Event!");
    }

}
