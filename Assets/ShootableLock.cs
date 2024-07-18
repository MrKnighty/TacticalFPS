using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ShootableLock : BodyPartDamageHandler
{
    Rigidbody rigidBody;
    [SerializeField] bool overrideForce = false;
    [SerializeField] float force;
    [SerializeField]Rigidbody doorRigidBody;
    override protected void Start()
    {
        if(!TryGetComponent<Rigidbody>(out rigidBody))
        {
            rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
        }

    }

    public override void DealDamage(float damage, float force = 0)
    {
        if(overrideForce)
            force = this.force;
        rigidBody.useGravity = true;
        doorRigidBody.isKinematic = false;
        // RaycastHit hit;
        // Physics.Raycast(transform.position, transform.forward, out hit);
        // Vector3 forceDir = 
        doorRigidBody.AddForceAtPosition(transform.forward * force, transform.position, ForceMode.Force);
    }
}
