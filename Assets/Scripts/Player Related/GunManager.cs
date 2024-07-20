using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GunManager : MonoBehaviour
{
    [SerializeField] BaseGun[] guns; // 0 is pistol, 1 is rifle, 2 is smg, 3 is shotgun
    [SerializeField] bool[] ownedGuns = { false, false, false, false };
    [SerializeField] PlayerSyringe playerSyringe;
    static public GunManager instance;
    bool switchingWeapon;

    [SerializeField] float switchSpeed;
    int activeWeapon = -1;
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

        foreach (AnimationState state in anim)
        {

            state.speed = 1.5f;
        }




    }
    public void LoadPersistantData()
    {

        guns[0].LoadAmmoValue(PresistantPlayerData.pistolMag, PresistantPlayerData.pistolAmmo);
        guns[1].LoadAmmoValue(PresistantPlayerData.rifleMag, PresistantPlayerData.rifleAmmo);
        guns[2].LoadAmmoValue(PresistantPlayerData.smgMag, PresistantPlayerData.smgAmmo);
        guns[3].LoadAmmoValue(PresistantPlayerData.shotgunShellsLoaded, PresistantPlayerData.shotgunAmmo);
        ownedGuns[0] = PresistantPlayerData.pistolUnlocked;
        ownedGuns[1] = PresistantPlayerData.rifleUnlocked;
        ownedGuns[2] = PresistantPlayerData.smgUnlocked;
        ownedGuns[3] = PresistantPlayerData.shotgunUnlocked;

        activeWeapon = PresistantPlayerData.currentWeapon;
        guns[activeWeapon].gameObject.SetActive(true); 
        guns[activeWeapon].UpdateUI();

       

    }
    public void WritePersistantData()
    {
        PresistantPlayerData.pistolMag = guns[0].currentAmmoInMagazine;
        PresistantPlayerData.pistolAmmo = guns[0].totalRemainingAmmo;

        PresistantPlayerData.rifleMag = guns[1].currentAmmoInMagazine;
        PresistantPlayerData.rifleAmmo = guns[1].totalRemainingAmmo;

        PresistantPlayerData.smgMag = guns[2].currentAmmoInMagazine;
        PresistantPlayerData.smgAmmo = guns[2].totalRemainingAmmo;

        PresistantPlayerData.shotgunShellsLoaded = guns[3].currentAmmoInMagazine;
        PresistantPlayerData.shotgunAmmo = guns[3].totalRemainingAmmo;

        PresistantPlayerData.pistolUnlocked = ownedGuns[0];
        PresistantPlayerData.rifleUnlocked = ownedGuns[1];
        PresistantPlayerData.smgUnlocked = ownedGuns[2];
        PresistantPlayerData.shotgunUnlocked = ownedGuns[3];

        PresistantPlayerData.currentWeapon = activeWeapon;
    }


    public void CancelADS()
    {
        guns[activeWeapon].CancelADS();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9) && DebugManager.DebugMode)
        {
            for (int i = 0; i < ownedGuns.Length; i++)
            {
                ownedGuns[i] = true;
            }
        }
        if (switchingWeapon)
            return;

        DebugManager.DisplayInfo("CWeaponI", "CWeaponI" + activeWeapon);
        DebugManager.DisplayInfo("LWeaponI", "LWeaponI" + lastGunIndex);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (activeWeapon == 0 || !ownedGuns[0])
                return;
            SwitchWeapon(0);


        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (activeWeapon == 1 || !ownedGuns[1])
                return;
            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (activeWeapon == 2 || !ownedGuns[2])
                return;

            SwitchWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (activeWeapon == 3 || !ownedGuns[3])
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
        switch (type)
        {
            case PickupType.SMG_AMMO:
                UICommunicator.refrence.PopupText("SMG Ammo + " + count, 1);
                guns[2].ReceiveAmmo(count);
                return;
            case PickupType.RIFLE_AMMO:
                UICommunicator.refrence.PopupText("Rifle Ammo + " + count, 1);
                guns[1].ReceiveAmmo(count);
                return;
            case PickupType.HEAL_SYRINGE:
                UICommunicator.refrence.PopupText("Gained + " + count + (count > 1 ? " Syringes" : "Syringe"), 1);
                GetComponent<PlayerSyringe>().GainSyringe(count);
                return;
            case PickupType.PISTOL_AMMO:
                UICommunicator.refrence.PopupText("Pistol Ammo + " + count, 1);
                guns[0].ReceiveAmmo(count);
                return;
            case PickupType.SHOTGUN_AMMO:
                UICommunicator.refrence.PopupText("Shotgun Ammo + " + count, 1);
                guns[3].ReceiveAmmo(count);
                return;


        }

    }

    public void ObtainWeapon(int index)
    {

        if (index == 0)
        {
            anim.Play(guns[0].name + "Equip");
            guns[0].gameObject.SetActive(true);
            activeWeapon = 0;
            ownedGuns[0] = true;
            return;
        }
        ownedGuns[index] = true;
        
        SwitchWeapon(index);
    }
}
