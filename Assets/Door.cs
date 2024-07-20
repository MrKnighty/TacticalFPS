using UnityEngine;

public class Door : Interactable
{
    bool playerHasNotUsedDoor;

    [SerializeField] bool doorLocked;
    [SerializeField] Rigidbody rb;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip openSound, lockedSound;
    [SerializeField] bool doorOpened;

    protected override void TriggerEvent()
    {
        if (doorOpened)
            return;

        

        if(doorLocked)
        {
            source.PlayOneShot(lockedSound, GameControllsManager.audioVolume);
            return;
        }

        rb.isKinematic = false;
        rb.AddForce(PlayerController.playerInstance.transform.forward * 50);
        source.PlayOneShot(openSound, GameControllsManager.audioVolume);
        doorOpened = true;
      
    }

    public void UnlockEvent()
    {
        doorLocked = false;
    }

    protected override void PreInteract()
    {
        base.PreInteract();
   
    }

}
