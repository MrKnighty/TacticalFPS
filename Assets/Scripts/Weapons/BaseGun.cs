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
    [SerializeField] GameObject mainCam;

    protected bool gunCanFire = true;
    protected bool canReload = true;
    protected bool reloading;

    protected bool isADSing;

    [Header("Audio")]
    [SerializeField] protected AudioSource source;
    [SerializeField] protected AudioClip fireSound;

    [Header("ADS Settings")]
    [SerializeField] Animator animator;

    [Header("Recoil Settings")]
    [SerializeField] Vector2[] recoilPoints; // Array of recoil points representing the amount of recoil per stage
    [SerializeField] float timeBetweeenRecoilPointDecay; // how much time it takes between each recoil point going down
    [SerializeField] float recoilEffectTime; // Duration of the recoil effect

    // Internal state variables
    int currentRecoilStage = 0; // Current stage of the recoil

  

 
    protected void Start()
    {
      
    }

   
    protected RaycastHit HitScan(Vector3 rayStart, Vector3 rayDirection)
    {
        RaycastHit hit;
        Physics.Raycast(rayStart, rayDirection, out hit);
        return hit;
    }

    protected void FixedUpdate()
    {

        DebugManager.DisplayInfo("cAmmo", "AmmoInMag" + currentAmmoInMagazine);
        DebugManager.DisplayInfo("rAmmo", "TotalAmmo" + totalRemainingAmmo);
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
        FireAudio();
        FireFVX();
        ShellEject();
        
        Recoil();

        animator.SetTrigger("Fire");

        currentAmmoInMagazine -= 1;
        if (currentAmmoInMagazine <= 0)
            gunCanFire = false;

        print("Shooting!");
        RaycastHit hit = HitScan(Camera.main.transform.position, Camera.main.transform.forward);


try{
      
            GameObject hitObject = HitScan(Camera.main.transform.position, Camera.main.transform.forward).transform.gameObject;
            DecalSpawn(hit.point);


            if (hitObject.GetComponent<DamageHandler>())
                hitObject.GetComponent<DamageHandler>().Damage(damage);

        }
        catch { }
      
    }

    protected void Recoil()
    {
        PlayerController.playerInstance.AddCameraRotation(recoilPoints[currentRecoilStage], recoilEffectTime, 25f); ///sssh ill make this magic nuber go away someday
        if (currentRecoilStage < recoilPoints.Length - 1)
            currentRecoilStage++;
        else
            currentRecoilStage--; // start looping
        recoilTimer = timeBetweeenRecoilPointDecay;


    }
    float recoilTimer;
    protected void RecoilDecay()
    {
        if (currentRecoilStage <= 0)
            return;

        recoilTimer -= Time.deltaTime;

        if(recoilTimer <= 0)
        {
            recoilTimer = timeBetweeenRecoilPointDecay;
            currentRecoilStage--;
        }

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

        RecoilDecay();

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
