using UnityEngine;

public class Pickup : Interactable
{
    public enum PickupType { SMG_AMMO, RIFLE_AMMO, HEAL_SYRINGE} 
    [SerializeField] PickupType type;

    protected override void TriggerEvent()
    {
        switch (type)
        {
            case PickupType.SMG_AMMO:
                break;
            case PickupType.RIFLE_AMMO:
                break;
            case PickupType.HEAL_SYRINGE:
                break;
        }
    }
}
