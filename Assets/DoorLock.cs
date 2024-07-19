using UnityEngine;

public class DoorLock : BodyPartDamageHandler
{
    [SerializeField] Door myDoor;
    public override void DealDamage(float damage, float force = 0)
    {
        
        myDoor.UnlockEvent();
        gameObject.AddComponent<Rigidbody>();
        base.DealDamage(damage, force);
    }
}
