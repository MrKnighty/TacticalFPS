using System.Collections;
using UnityEngine;

public class BasicShootAI : AIBase
{
    bool playerInSight;
    Transform playerPoint;
    protected override void Start()
    {
        base.Start();
        mainLoop();
    }
    void mainLoop()
    {
        if (currentState == AIStates.Patrol)
            StartCoroutine(Patrol());
    }
    void SwitchStates(AIStates state)
    {
        switch (state)
        {
            case AIStates.Patrol:
                break;
            case AIStates.Gaurd: 
                break;
            case AIStates.Aggro:
                currentState = state;
                StartCoroutine(AggroLoop());
                break;
        }

    }

    IEnumerator AggroLoop()
    {
        StartCoroutine(ShootLoop());
        if(!inCover)
            yield return MoveToCover();
        while(playerPoint)
        {
            RotateTowardsTarget();
            yield return null;
        }
        yield return Timer(guardTime);

        StartCoroutine(AggroLoop());
    }
    IEnumerator MoveToCover()
    {
        Vector3 coverPoint = Vector3.zero;
        if ((coverPoint = GetCoverPointOnNavMesh(FindClosestCover())) != Vector3.zero)
        {
            agent.isStopped = false;
            agent.SetDestination(coverPoint);
            while(transform.position !=  agent.destination) 
                yield return null;
            inCover = true; 
            agent.isStopped = true;
        }
    }
    IEnumerator ShootLoop()
    {
        while(playerPoint)
        {
            if(ammo == 0)
                yield return Reload();
            if(playerPoint == null)
            {
                break;
            }
            ShootAtTarget(playerPoint.position, shootRadius);
            yield return Timer(attackSpeed);
            ammo--;
        }
    }
    IEnumerator Reload()
    {
        //Play Reload Animation
        yield return Timer(reloadTime);
        ammo = ammoCap;

    }
    void RotateTowardsTarget()
    {
        Quaternion dir = Quaternion.LookRotation(new Vector3(playerPoint.position.x, transform.position.y, playerPoint.position.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, dir, patrolTurnSpeed);
         
    }

    float aggroCheckTimer = 0;
    float timers = 0;
    void AggroCheck()
    {
        if (aggroCheckTimer < 0.1f)
        {
            aggroCheckTimer += Time.deltaTime;
            return;
        }
        aggroCheckTimer = 0;
        playerPoint = CanSeePlayer();
        if (currentState != AIStates.Aggro)
        {
            SwitchStates(AIStates.Aggro);
        }

    }
    private void Update()
    {
        AggroCheck();
    }
}
