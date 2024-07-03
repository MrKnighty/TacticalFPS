using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GunManager : MonoBehaviour
{
    [SerializeField] BaseGun[] guns; // 0 is smg, 1 is rifle
    [SerializeField] PlayerSyringe playerSyringe;
    static public GunManager instance;
    bool switchingWeapon;
    Animator animator;
    [SerializeField] float switchSpeed;
    int activeWeapon = 0;
    int lastGunIndex = 0;
    private void Start()
    {
        animator = GetComponent<Animator>();
        instance = this;

        //reset static gun values
        BaseGun.isADSing = false;
        BaseGun.playerInMidAir = false;
        BaseGun.fireForbidden = false;

        StartCoroutine(PrepairWeapons());
        Time.timeScale = 0.25f;
    }

    IEnumerator PrepairWeapons()
    {
        foreach (BaseGun gun in guns)
        {
            if (gun.gameObject.activeSelf)
                continue;
            gun.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            gun.gameObject.SetActive(false);
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

            if (!guns[activeWeapon].ReadyToSwitch())
                return;

            guns[activeWeapon].StopReloading();

            animator.enabled = true;
            BaseGun.fireForbidden = true;
            animator.SetTrigger("From" + guns[activeWeapon].name);
            animator.SetTrigger("ToMP7");
            lastGunIndex = activeWeapon;
           
            switchingWeapon = true;
            activeWeapon = 0;

            CancelADS();

  
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (activeWeapon == 1)
                return;

            if (!guns[activeWeapon].ReadyToSwitch())
                return;

            guns[activeWeapon].StopReloading();
            animator.enabled = true;
            BaseGun.fireForbidden = true;
            animator.SetTrigger("From" + guns[activeWeapon].name);
            animator.SetTrigger("ToSniper");
            switchingWeapon = true;
            lastGunIndex = activeWeapon;
            activeWeapon = 1;

            CancelADS();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (activeWeapon == 2)
                return;

            if (!guns[activeWeapon].ReadyToSwitch())
                return;

            guns[activeWeapon].StopReloading();
            animator.enabled = true;
            BaseGun.fireForbidden = true;
            animator.SetTrigger("From" + guns[activeWeapon].name);
            animator.SetTrigger("ToShotgun");
            switchingWeapon = true;
            lastGunIndex = activeWeapon;
            activeWeapon = 2;

            CancelADS();
        }
    }


    
  
  
    public void SwitchGun() // controlled by animator
    {
        
        guns[lastGunIndex].gameObject.SetActive(false);
        guns[activeWeapon].gameObject.SetActive(true);
        guns[activeWeapon].UpdateUI();
        print("Switched gun");
        
    }
    public void SwitchFinished()//controlled by animator
    {
        switchingWeapon = false;
        BaseGun.fireForbidden = false;
        animator.enabled = false;
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
