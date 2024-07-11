using System;
using UnityEngine;

public class BodyPartDamageHandler : MonoBehaviour
{
    [SerializeField] BodyParts bodyPart;
    DamageHandler damageHandler;
    protected virtual void Start()
    {
        try 
        {
            damageHandler = GetComponentInParent<DamageHandler>();
        }
        catch
        {
            Console.Error.WriteLine("Damage Handler Not Found In Parent: " + transform.parent.name);
        }
    }
    public virtual void DealDamage(float damage, float force = 0)
    {
        damageHandler.Damage(damage, bodyPart);
    }
}
