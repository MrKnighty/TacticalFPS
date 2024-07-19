using UnityEngine;

public class PresistantPlayerData : MonoBehaviour
{
    static public bool playerPresistant;
    static public float health;
    static public int  pistolAmmo, smgAmmo, rifleAmmo, shotgunAmmo, syringes;
    static public int pistolMag, smgMag, rifleMag,shotgunShellsLoaded;
    static public bool pistolUnlocked, rifleUnlocked, smgUnlocked, shotgunUnlocked;
    static public int currentWeapon;

    private void Start()
    {
        if (playerPresistant)
        {
            print("Loading Player Data!");
            SendMessage("LoadPersistantData");
        }
        else
        {
            print("No Data To load!");
        }
            

    }

    public void Save()
    {
        SendMessage("WritePersistantData");
        playerPresistant = true;
    }
}
