using System.Collections;
using UnityEngine;

public class BasicShootAI : AIBase
{
    Transform playerVisablePoint;
    bool playerInSight;
    protected override void Start()
    {
       base.Start();

    }
    void mainLoop()
    {

    }
   

    float aggroCheckTimer = 0;
    float timer = 0;
    private void Update()
    {
        print(CanSeePlayer());
        if (timer < attackSpeed)
        {
            timer += Time.deltaTime;
            return;
        }
        timer = 0;
        Transform point = CanSeePlayer();
        if (point)
            ShootAtTarget(point.position, shootRadius);

        if (aggroCheckTimer < 0.1f)
        {
            aggroCheckTimer += Time.deltaTime;
            return;
        }
        aggroCheckTimer = 0;
        playerVisablePoint = CanSeePlayer();
    }
}
