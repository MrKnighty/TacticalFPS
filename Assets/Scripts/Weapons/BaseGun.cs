using System;
using UnityEngine;
using System.Collections;

public class BaseGun : MonoBehaviour
{
    [SerializeField] protected GameObject muzzlePoint;
    [SerializeField] protected float magazineSize; // max amount of ammo that can be in magazine
    [SerializeField] protected float currentAmmoInMagazine;// how many bullets ready to fire
    [SerializeField] protected float totalRemainingAmmo; // remaining ammo not in magazine
    [SerializeField] protected float reloadTime;
    [SerializeField] protected float damage;

    [SerializeField] GameObject decal;
    [SerializeField] GameObject hitParticle;

    protected bool gunCanFire = true;
    protected bool canReload = true;
    protected bool reloading;

    protected bool isADSing;

    [Header("Audio")]
    [SerializeField] protected AudioSource source;
    [SerializeField] protected AudioClip fireSound;

    [Header("ADS Settings")]
    [SerializeField] Animator animator;
   

    protected void Start()
    {
     
    }

    protected RaycastHit HitScan(Vector3 rayStart, Vector3 rayDirection)
    {
        RaycastHit hit;
        Physics.Raycast(rayStart, rayDirection, out hit);
        return hit;

    }

    protected void FireAudio()
    {
        source.PlayOneShot(fireSound);
    }

    protected void FireFVX()
    {

    }

    protected void ShellEject()
    {

    }

    protected void DecalSpawn(Vector3 hitPos)
    {
        Instantiate(decal, hitPos, Quaternion.identity);
        Instantiate(hitParticle, hitPos, Quaternion.identity).transform.LookAt(transform);
        
    }

    protected void Reload()
    {
        if (totalRemainingAmmo <= 0)
        {
            canReload = false;
            return;
        }
        animator.SetTrigger("Reload");
        StartCoroutine(ReloadEvent());
    }

    protected IEnumerator ReloadEvent()
    {
        gunCanFire = false;
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
       
        totalRemainingAmmo += currentAmmoInMagazine;
        if (totalRemainingAmmo >= magazineSize)
        {
            currentAmmoInMagazine = magazineSize;
            totalRemainingAmmo -= magazineSize;
        }
        else
        {
            currentAmmoInMagazine = totalRemainingAmmo;
            totalRemainingAmmo = 0;
        }
        gunCanFire = true;
        reloading = false;
        yield return null;
    }

    protected void FireEvent() // everything that should happen when the gun fires
    {
        print("Shooting!");
        RaycastHit hit = HitScan(Camera.main.transform.position, Camera.main.transform.forward);
        GameObject hitObject = HitScan(Camera.main.transform.position, Camera.main.transform.forward).transform.gameObject;
        if(hitObject != null)
            print(hitObject);

        if (hitObject.GetComponent<DamageHandler>())
            hitObject.GetComponent<DamageHandler>().Damage(damage);
        

        FireAudio();
        FireFVX();
        ShellEject();
        DecalSpawn(hit.point);

        animator.SetTrigger("Fire");

        currentAmmoInMagazine -= 1;
        if (currentAmmoInMagazine <= 0)
            gunCanFire = false;
    }

    float ADSprogress = 0;

    protected IEnumerator ADS()
    {
        yield break;    
    }

  

    protected void Update()
    {
        if (reloading)
            return;

        if (Input.GetKeyDown(KeyCode.R) && canReload)
        {
            Reload();
        }
        if (Input.GetMouseButtonDown(0) && gunCanFire)
            FireEvent();

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("ADS", true);
          
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("ADS", false);

        }
    }
}
