using System;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class BaseGun : MonoBehaviour
{
    [SerializeField]  GameObject muzzlePoint;
    [SerializeField]  float magazineSize; // max amount of ammo that can be in magazine
    float currentAmmoInMagazine;// how many bullets ready to fire
    [SerializeField]  float totalRemainingAmmo; // remaining ammo not in magazine
    [SerializeField] float maxAmmo;
    [SerializeField]  float reloadTime;
    [SerializeField]  float damage;


    [SerializeField] GameObject mainCam;

    protected bool gunCanFire = true;
    protected bool canReload = true;
    protected bool reloading;

    public static bool isADSing;

    [Header("Audio")]
    [SerializeField] protected AudioSource source;
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip reloadSound;

    [Header("ADS Settings")]
    [SerializeField] Animator animator;

    [Header("Recoil Settings")]
    [SerializeField] Vector2[] recoilPoints; // Array of recoil points representing the amount of recoil per stage
    [SerializeField] int subPoints;
    [SerializeField] float timeBetweeenRecoilPointDecay; // how much time it takes between each recoil point going down
    [SerializeField] float recoilEffectTime; // Duration of the recoil effect
    [SerializeField] float recoilMultiplyer; // fast way to increase recoil without having to rewrite numbers
    [SerializeField] float notADSingRecoilMultiplyer; // this is a addative recoil, not a adtional multiplyer
    [SerializeField] float midAirRecoilMultiplyer;
    [SerializeField] bool randomHipFire;
    [SerializeField] float randomHitRadius;

    [SerializeField] bool isAutomatic;
    [SerializeField] float fireRate;

    [Header("EffectSettings")]
    
    [SerializeField] GameObject lightObject;
    [SerializeField] float lightStayOnTime;
    [SerializeField] GameObject bulletCasingSpawnPoint;
    [SerializeField] GameObject bulletCasingToSpawn;
    [SerializeField] int maxShells;
    [SerializeField] float shellEjectVelocity;
    [SerializeField] float ShellEjectVelocityRandomOffset;
    [SerializeField] GameObject flashLight;
    [SerializeField] ParticleSystem muzzleFlashFX;

    public static bool adsForbidden;
    public static bool playerInMidAir;
    public static bool fireForbidden;

    

   

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
      
        shells = new GameObject[maxShells];

        for (int i = 0; i < maxShells; i++)
        {
      
            shells[i] = Instantiate(bulletCasingToSpawn, new Vector3(UnityEngine.Random.Range(-1000, -10050), UnityEngine.Random.Range(-1000, -10050), UnityEngine.Random.Range(-1000, -10050)), quaternion.identity);
        }

        currentAmmoInMagazine = magazineSize;
    }


    public void ReceiveAmmo(float count)
    {
        totalRemainingAmmo += count;
        if (totalRemainingAmmo < maxAmmo + (magazineSize - currentAmmoInMagazine)) // temporarily increase max ammo if player does not have full mag
            totalRemainingAmmo = maxAmmo;
        UpdateUI();
    }


    public void UpdateUI()
    {
     
        UICommunicator.UpdateUI("Ammo Text", currentAmmoInMagazine + " / " + totalRemainingAmmo);

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

    protected void FireAudio(RaycastHit hit)
    {
        AudioClip[] sounds = MaterialPropertiesManager.GetBulletImpactSounds(hit.transform.gameObject);
        AudioSource.PlayClipAtPoint(sounds[UnityEngine.Random.Range(0, sounds.Length - 1)], hit.point, 1.5f * UICommunicator.audioLevel);
  
      
    }

    protected void FireFVX()
    {
        lightObject.SetActive(true);
        Invoke("StopLight", lightStayOnTime);
        muzzleFlashFX.Play(true);

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

    protected void DecalSpawn(RaycastHit hit)
    {
        GameObject decal = MaterialPropertiesManager.GetDecal(hit.transform.gameObject);
        GameObject hitParticle = MaterialPropertiesManager.GetHitParticle(hit.transform.gameObject);

        Instantiate(decal, hit.point, Quaternion.identity).transform.LookAt(transform);
        Instantiate(hitParticle, hit.point, Quaternion.identity);


    }

    protected void Reload()
    {
        if (currentAmmoInMagazine >= magazineSize)
            return;
        if (totalRemainingAmmo <= 0)
        {
            canReload = false;
            return;
        }
        source.PlayOneShot(reloadSound, UICommunicator.audioLevel);
        animator.SetTrigger("Reload");
        StartCoroutine(ReloadEvent());
    }
    public void StopReloading()
    {
        StopAllCoroutines();
        reloading = false;
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
        UpdateUI();
        yield return null;
    }

    protected void BulletInpact(RaycastHit hit)
    {
        DecalSpawn(hit);
        FireAudio(hit);
    
    }

    protected void FireEvent() // everything that should happen when the gun fires
    {
        shotsFired++;
        lastTimeSinceFired = fireRate;
        source.PlayOneShot(fireSound, 0.5f * UICommunicator.audioLevel);
        FireFVX();
        BulletCasingEject();
      
        
      

        animator.SetTrigger("Fire");

        currentAmmoInMagazine -= 1;
        Vector3 offset = Vector3.zero;
        if (randomHipFire && !isADSing)
        {
            offset.x = UnityEngine.Random.Range(-randomHitRadius, randomHitRadius);
            offset.y = UnityEngine.Random.Range(-randomHitRadius, randomHitRadius);
        }

        RaycastHit hit = HitScan(Camera.main.transform.position, Camera.main.transform.forward + offset);
        Recoil();
        BulletInpact(hit);

        try
        {
            
        GameObject hitObject = HitScan(Camera.main.transform.position, Camera.main.transform.forward).transform.gameObject;
        


        if (hitObject.GetComponent<BodyPartDamageHandler>())
        {
            hitObject.GetComponent<BodyPartDamageHandler>().DealDamage(damage);
            shotsHit++;
        }
        
      
        }
        
        catch { }

        DebugManager.DisplayInfo("ACC", "Accuracy:" + shotsHit / shotsFired);
       
        if(currentAmmoInMagazine <= 0)
        {
            if (canReload)
                Reload();
            else
                 gunCanFire = false;
        }

        UpdateUI();
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

    public bool ReadyToSwitch()
    {
        return !reloading;
    }

  

    protected IEnumerator ADS()
    {
        yield break;    
    }

    protected void Update()
    {
        if (reloading || UICommunicator.gamePaused || fireForbidden)
            return;

        lastTimeSinceFired -= Time.deltaTime;

        RecoilDecay();

        if (Input.GetKeyDown(KeyCode.R) && canReload)
        {
            Reload();
        }
        if (lastTimeSinceFired <= 0 && gunCanFire)
        {
            if (Input.GetMouseButton(0) && isAutomatic)
                FireEvent();
            else if (Input.GetMouseButtonDown(0))
                FireEvent();
        }
            

        if (Input.GetMouseButtonDown(1) && !adsForbidden)
        {
            isADSing = true;
            animator.SetBool("ADS", true);
          
        }
        else if (Input.GetMouseButtonUp(1) || adsForbidden)
        {
            isADSing = false;
            animator.SetBool("ADS", false);

        }
        PlayerController.playerInstance.isAdsIng = isADSing;

        if (Input.GetKeyDown(KeyCode.F))
            flashLight.SetActive(!flashLight.activeSelf);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 1000);
    }
}
