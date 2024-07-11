using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] int weaponID;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            other.GetComponent<GunManager>().ObtainWeapon(weaponID);
            Destroy(gameObject);
        }

    }
}
