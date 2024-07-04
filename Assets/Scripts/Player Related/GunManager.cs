using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GunManager : MonoBehaviour
{
    [SerializeField] BaseGun[] guns; // 0 is smg, 1 is rifle
    [SerializeField] PlayerSyringe playerSyringe;
    static public GunManager instance;
    bool switchingWeapon;

    [SerializeField] float switchSpeed;
    int activeWeapon = 0;
    int lastGunIndex = 0;

  
    [SerializeField] Animation anim;
    private void Start()
    {
    
        instance = this;

    
        BaseGun.isADSing = false;
        BaseGun.playerInMidAir = false;
        BaseGun.fireForbidden = false;

        

        foreach (BaseGun gun in guns)
        {
            if (gun.gameObject.activeSelf)
                continue;

            gun.Initilize();
        }

    }

    public void CancelADS()
    {
        guns[activeWeapon].CancelADS();
    }

    private void Update()
    {
        if (switchingWeapon)
            return;

        DebugManager.DisplayInfo("CWeaponI", "CWeaponI" + activeWeapon);
        DebugManager.DisplayInfo("LWeaponI", "LWeaponI" + lastGunIndex);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (activeWeapon == 0)
                return;
            SwitchWeapon(0);


        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (activeWeapon == 1)
                return;
            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (activeWeapon == 2)
                return;

            SwitchWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (activeWeapon == 3)
                return;

            SwitchWeapon(3);
        }
    }

    void SwitchWeapon(int newWeapon)
    {
        if (activeWeapon == newWeapon)
            return;

        if (!guns[activeWeapon].ReadyToSwitch())
            return;

        guns[activeWeapon].StopReloading();

        BaseGun.fireForbidden = true;


        anim.Play(guns[activeWeapon].name + "Deaquip");
        switchingWeapon = true;
        lastGunIndex = activeWeapon;
        activeWeapon = newWeapon;
        guns[activeWeapon].GetComponent<Animator>().StopPlayback();

        CancelADS();
    }


    
  
  
    public void SwitchGun() // controlled by animator
    {
        guns[lastGunIndex].GetComponent<Animator>().SetTrigger("StopADS");
        guns[lastGunIndex].gameObject.SetActive(false);
        guns[activeWeapon].gameObject.SetActive(true);
        guns[activeWeapon].UpdateUI();

        anim.Play(guns[activeWeapon].name + "Equip");
        print("Switched gun");
        
    }
    public void SwitchFinished()//controlled by animator
    {
        switchingWeapon = false;
        BaseGun.fireForbidden = false;
  
    }
    public void GiveAmmo(PickupType type, int count)
    {
        switch(type)
        {
            case PickupType.SMG_AMMO:
                UICommunicator.refrence.PopupText("SMG Ammo + " + count, 1);
                guns[0].ReceiveAmmo(count);
                return;
            case PickupType.RIFLE_AMMO:
                UICommunicator.refrence.PopupText("Rifle Ammo + " + count, 1);
                guns[1].ReceiveAmmo(count);
                return;
            case PickupType.HEAL_SYRINGE:
                UICommunicator.refrence.PopupText("Gained + " + count + (count > 1 ? " Syringes" : "Syringe"), 1);
                GetComponent<PlayerSyringe>().GainSyringe(count);

                return;
        }
            
    }
}
