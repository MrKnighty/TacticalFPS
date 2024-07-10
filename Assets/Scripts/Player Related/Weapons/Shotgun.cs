using UnityEngine;
using System.Collections.Generic;

public class Shotgun : BaseGun
{
    [Header("Shotgun Settings")]
    [SerializeField] float pelletCount;
    [SerializeField] Vector2 spreadArea;
    [SerializeField] float minDistanceBetweenPellets;

    protected override void Update()
    {
        if (UICommunicator.gamePaused)
            return;

        lastTimeSinceFired -= Time.deltaTime;

        RecoilDecay();

        if (reloading || fireForbidden)
            return;

        if (TryReload())
            Reload();

        if (TryFire())
            ShotgunSpread();

        ADS();

        PlayerController.playerInstance.isAdsIng = isADSing;
        Flashlight();

        //debug

    
    }

    void ShotgunSpread()
    {
        bulletPos = new();
        FireEvent(true);
        for(int i = 0; i < pelletCount - 1; i++)
        {
            FireEvent(false);
            print(i);
        }
    }

    List<Vector2> bulletPos = new();
   
    protected override RaycastHit HitScan(Vector3 rayStart, Vector3 rayDirection)
    {
        int i = 0;
        while(true)
        {
            i++;
            if (i >= 5)
            {
                print("WARNING: minDistanceBetweenPellets is too high! cannot find new pos");
                return new RaycastHit();
            }

            if (bulletPos.Count == 0) // garantee that the first shot will hit
            {
                Physics.Raycast(rayStart, rayDirection, out RaycastHit hit);
                bulletPos.Add(rayDirection);
                return hit;
                
            }

            Vector2 offset = new Vector2(Random.Range(-spreadArea.x, spreadArea.x), Random.Range(-spreadArea.y, spreadArea.y)) ;
           
            foreach (Vector2 p in bulletPos)
            {
                if (Vector2.Distance(offset, p) < minDistanceBetweenPellets)
                    break;
                else
                {
                    bulletPos.Add(p);
                    Vector3 offsetRay = rayDirection;
                
                    offsetRay += Camera.main.transform.right * offset.x + Camera.main.transform.up * offset.y;
                    Physics.Raycast(rayStart, offsetRay, out RaycastHit hit);
                    return hit;
                }
            }
        }
    }
  
}
