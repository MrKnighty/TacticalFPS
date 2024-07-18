using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] int weaponID;
    [SerializeField] bool destroy;
    bool trigger;
    private void OnTriggerEnter(Collider other)
    {
        if(trigger)
        return;
        if(other.transform.tag == "Player")
        {
            trigger = true;
            other.GetComponent<GunManager>().ObtainWeapon(weaponID);
            if(destroy)Destroy(gameObject);
            else
            {
                foreach(Transform obj in transform.GetComponentInChildren<Transform>())
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }

    }
}
