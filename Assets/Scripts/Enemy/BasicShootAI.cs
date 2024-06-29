using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicShootAI : AIBase
{
    [SerializeField]bool playerInSight;
    [SerializeField] bool aggressive;
    Vector3 lastSeenPlayerPosition;
    Transform playerPoint;
    Transform currentCoverTransform;
    [Header("Chase")]
    [SerializeField] float searchIterations = 5;
    [SerializeField] float searchRange = 5;
    [SerializeField] ParticleSystem muzzleFX;
    protected override void Start()
    {
        base.Start();
        SwitchStates(currentState);
    }
    void SwitchStates(AIStates state)
    {
        StopAllCoroutines();
        agent.isStopped = true;
        switch (state)
        {
            case AIStates.Patrol:
                currentState = state;
                if(patrolPoints.Length > 1)
                StartCoroutine(Patrol());
                break;
            case AIStates.Gaurd: 
                currentState = state;
                if (!inCover)
                {
                    Vector3 point = GetCoverPointOnNavMesh(FindClosestCover());
                    if (point != Vector3.zero)
                    {
                        MoveToDestination(GetCoverPointOnNavMesh(FindClosestCover()));
                        inCover = true;
                    }
                }
                break;
            case AIStates.Aggro:
                currentState = state;
                StartCoroutine(Aggro());
                StartCoroutine(Shoot());
                break;
            case AIStates.Chase:
                currentState = state;

                StartCoroutine(Chase());
                break;
        }

    }
    public override void DamageTrigger()
    {
        if(currentState != AIStates.Aggro)
        {
            SwitchStates(AIStates.Aggro);
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Destroy(gameObject);
            return;
        }
        playerPoint = CanSeePlayer();
        if(playerPoint)
        {
            if(!playerInSight)
            {
                playerInSight = true;
                agent.updateRotation = false;
            }
           if(currentState != AIStates.Aggro)
            {
                SwitchStates(AIStates.Aggro);
            }
        }
        else if(playerInSight)
        {
            playerInSight = false;
            agent.updateRotation = true;
            lastSeenPlayerPosition = playerTransform.position;

        }
    }

    IEnumerator Aggro()
    {
        while(currentState == AIStates.Aggro)
        {
            if(!playerPoint)
            {
                yield return Timer(guardTime);
                if(aggressive)
                {
                    SwitchStates(AIStates.Chase);
                    yield break;
                }
                else
                {
                    SwitchStates(AIStates.Gaurd);
                    yield break;
                }
            }
            if (!inCover)
            {
                Vector3 point = GetCoverPointOnNavMesh(FindClosestCover());
                print(point);
                if(point != Vector3.zero)
                {
                    yield return MoveToDestination(GetCoverPointOnNavMesh(FindClosestCover()));
                    inCover = true;
                }
            }
            yield return null;
        }
    }
    IEnumerator Shoot()
    {
        bool lostSight = true;
        while(currentState == AIStates.Aggro)
        {
            Vector3 lookDir = playerTransform.position;
            lookDir.y = transform.position.y;
            transform.LookAt(lookDir);
            if(lostSight)
            {
                lostSight = false;
                yield return Timer(reactionSpeed);
            }
            if(ammo <= 0)
            {
                yield return Timer(reloadTime);
                ammo = ammoCap;
            }
            if (!playerPoint)
            {
                lostSight = true;
                yield return null;
                continue;
            }
            if(ShootAtTarget(playerPoint.position, shootRadius))
            {
                playerDamageHandler.Damage(damage);
            }
            if (muzzleFX)
                muzzleFX.Play(true);
            ammo--;
            yield return Timer(attackSpeed);
        }

    }
    IEnumerator Gaurd()
    {
        yield return null;
    }
    IEnumerator Chase()
    {
        
        NavMeshHit hit;
        NavMesh.SamplePosition(lastSeenPlayerPosition, out hit, 5, NavMesh.AllAreas);
        yield return MoveToDestination(hit.position);
        searchIterations = 5;
        while(searchIterations > 0)
        {
            Vector3 point = new Vector3(playerTransform.position.x + Random.Range(-searchRange, searchRange), playerTransform.position.y + Random.Range(-searchRange, searchRange), playerTransform.position.z + Random.Range(-searchRange, searchRange));
            if(NavMesh.SamplePosition(point, out hit, 5, NavMesh.AllAreas))
            {
                yield return MoveToDestination(hit.position);
                searchIterations--;
            }
           yield return null;
        }
    }
    IEnumerator Search()
    {
        yield return null;
    }
    IEnumerator MoveToDestination(Vector3 point)
    {
        agent.isStopped = false;
        agent.destination = point;
        while (Vector3.Distance(transform.position, point) > 0.2f)
        {
            yield return null;
        }
        agent.isStopped = true;
    }

}
