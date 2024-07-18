using UnityEngine;
public enum PickupType {PISTOL_AMMO, RIFLE_AMMO, SHOTGUN_AMMO, SMG_AMMO, HEAL_SYRINGE }
public class Pickup : Interactable
{
  
    [SerializeField] PickupType type;
    [SerializeField] int amount;

    protected override void TriggerEvent()
    {
        GunManager.instance.GiveAmmo(type, amount);
    }
}
