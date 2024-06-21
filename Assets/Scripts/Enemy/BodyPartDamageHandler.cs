using System;
using UnityEngine;

public class BodyPartDamageHandler : MonoBehaviour
{
    [SerializeField] BodyParts bodyPart;
    DamageHandler damageHandler;
    void Start()
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
    public void DealDamage(float damage)
    {
        damageHandler.Damage(damage, bodyPart);
    }
}
