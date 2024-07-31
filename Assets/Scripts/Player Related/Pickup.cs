using UnityEngine;
public enum PickupType {PISTOL_AMMO, RIFLE_AMMO, SHOTGUN_AMMO, SMG_AMMO, HEAL_SYRINGE }
public class Pickup : Interactable
{
  
    [SerializeField] PickupType type;
    [SerializeField] int amount;

    [SerializeField] GameObject pistol, rifle, smg, shotgun,health;
    protected override void TriggerEvent()
    {
        GunManager.instance.GiveAmmo(type, amount);
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        pistol.SetActive(false);
        rifle.SetActive(false);
        smg.SetActive(false);
        shotgun.SetActive(false);
        health.SetActive(false);

        switch (type)
        {
            
            case PickupType.PISTOL_AMMO:
                pistol.SetActive(true);
                break;
            case PickupType.RIFLE_AMMO:
                rifle.SetActive(true); 
                break;
            case PickupType.SHOTGUN_AMMO:
                shotgun.SetActive(true); 
                break;
            case PickupType.SMG_AMMO:
                smg.SetActive(true); 
                break;
            case PickupType.HEAL_SYRINGE:
                health.SetActive(true); 
                break;




        }

    }
}
