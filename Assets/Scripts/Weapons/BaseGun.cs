using System;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;

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
    [SerializeField] int subPoints;
    [SerializeField] float timeBetweeenRecoilPointDecay; // how much time it takes between each recoil point going down
    [SerializeField] float recoilEffectTime; // Duration of the recoil effect
    [SerializeField] float recoilMultiplyer; // fast way to increase recoil without having to rewrite numbers
    [SerializeField] float notADSingRecoilMultiplyer; // this is a addative recoil, not a adtional multiplyer

    [SerializeField] bool isAutomatic;
    [SerializeField] float fireRate;

    [Header("EffectSettings")]
    
    [SerializeField] GameObject lightObject;
    [SerializeField] float lightStayOnTime;
    [SerializeField] GameObject bulletCasingSpawnPoint;
    [SerializeField] GameObject bulletCasingToSpawn;
    [SerializeField] float maxShells;
    [SerializeField] float shellEjectVelocity;
    [SerializeField] float ShellEjectVelocityRandomOffset;

    GameObject[] shells;
    int currentShellIndex;

    float lastTimeSinceFired;

    // Internal state variables
    int currentSubRecoilStage = 0;
    int currentRecoilStage = 0; // Current stage of the recoil

    float shotsFired;
    float shotsHit;

   
    void Start()
    {
        shells = new GameObject[subPoints];

        for (int i = 0; i < subPoints; i++)
        {
            shells[i] = Instantiate(bulletCasingToSpawn, new Vector3(-1000, -1000,-1000), quaternion.identity);
        }
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

        DebugManager.DisplayInfo("rStage", "rStage" + currentRecoilStage);
        DebugManager.DisplayInfo("rSubStage", "rSubStage" + currentSubRecoilStage);
    }

    protected void FireAudio()
    {
        source.PlayOneShot(fireSound);
    }

    protected void FireFVX()
    {
        lightObject.SetActive(true);
        Invoke("StopLight", lightStayOnTime);
    }

    void StopLight()
    {
        lightObject.SetActive(false);
    }

    protected void BulletCasingEject()
    {
        GameObject shell = shells[currentShellIndex];
        shell.transform.position = bulletCasingSpawnPoint.transform.position;
        shell.GetComponent<Rigidbody>().linearVelocity = bulletCasingSpawnPoint.transform.forward * (shellEjectVelocity + UnityEngine.Random.Range(-ShellEjectVelocityRandomOffset, ShellEjectVelocityRandomOffset));
       
        currentShellIndex ++;
        if(currentShellIndex >= shells.Length -1)
           currentShellIndex = 0;

        if(Input.GetKey(KeyCode.LeftShift))
            Debug.Break();
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
        shotsFired++;
        lastTimeSinceFired = fireRate;
        FireAudio();
        FireFVX();
        BulletCasingEject();
        
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


        if (hitObject.GetComponent<BodyPartDamageHandler>())
            hitObject.GetComponent<BodyPartDamageHandler>().DealDamage(damage);
            shotsHit++;
        }
        catch { }

        DebugManager.DisplayInfo("ACC", "Accuracy:" + shotsHit / shotsFired);
      
    }


    protected void Recoil()
    {
        currentSubRecoilStage++;
        if(currentSubRecoilStage >= subPoints)
        {
            currentSubRecoilStage = 0;
            if (currentRecoilStage < recoilPoints.Length - 2)
                currentRecoilStage++;
            else
                currentRecoilStage -= 2;


        }
        Vector2 recoilVector = Vector2.Lerp(recoilPoints[currentRecoilStage], recoilPoints[currentRecoilStage + 1], currentSubRecoilStage / subPoints);
        PlayerController.playerInstance.AddCameraRotation(recoilVector, recoilEffectTime, recoilMultiplyer + (isADSing ? 0 : notADSingRecoilMultiplyer)); ///sssh ill make this magic nuber go away someday // i did it :-) 
      
        recoilTimer = timeBetweeenRecoilPointDecay;


    }
    float recoilTimer;
    protected void RecoilDecay()
    {
        if (currentRecoilStage < 0 && currentSubRecoilStage <= 0)
            return;

        recoilTimer -= Time.deltaTime;

        if(recoilTimer <= 0)
        {

            currentSubRecoilStage--;
            if(currentSubRecoilStage <= 0)
            {
                if(currentRecoilStage > 0)
                     currentRecoilStage--;
                if (currentRecoilStage > 0)
                    currentSubRecoilStage = subPoints;
                else
                {
                    currentSubRecoilStage = 0;
                    return;
                }
            }
            recoilTimer = timeBetweeenRecoilPointDecay;
     
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

        lastTimeSinceFired -= Time.deltaTime;

        RecoilDecay();

        if (Input.GetKeyDown(KeyCode.R) && canReload)
        {
            Reload();
        }
        if (lastTimeSinceFired <= 0)
        {
            if (Input.GetMouseButton(0) && isAutomatic)
                FireEvent();
            else if (Input.GetMouseButtonDown(0))
                FireEvent();
        }
            

        if (Input.GetMouseButtonDown(1))
        {
            isADSing = true;
            animator.SetBool("ADS", true);
          
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isADSing = false;
            animator.SetBool("ADS", false);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 1000);
    }
}
