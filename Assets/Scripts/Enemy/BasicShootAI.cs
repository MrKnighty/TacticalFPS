using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicShootAI : AIBase
{
    [Header("SideStepping")]
    [SerializeField] float sideStepMaxDistance;
    [SerializeField, Range(0, 100)] float sideStepChance = 50;
    [SerializeField] float sideStepCoolDownTime = 15f;
    bool sideStepping;
    [SerializeField] bool aggressive;
    Vector3 lastSeenPlayerPosition;
    Transform currentCoverTransform;
    [Header("Chase")]
    [SerializeField] float searchIterations = 5;
    [SerializeField] float searchRange = 5;
    [SerializeField, Tooltip("In Degrees")] float maxSnapRotateSpeed = 15;
    [SerializeField] ParticleSystem muzzleFX;
<<<<<<< Updated upstream
=======
    [SerializeField] Animator rigAnimator;
    //Triggers
    bool playerInSightTrigger;
>>>>>>> Stashed changes
    protected override void Start()
    {
        base.Start();
        SwitchStates(currentState);
    }
<<<<<<< Updated upstream
=======
    void ExitState()
    {
        switch (currentState)
        {
            case AIStates.Aggro:
                rigAnimator.SetBool("Aggro", false);
            break;
            
        }
    }
>>>>>>> Stashed changes
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
        if(!inCover && canSideStep) //SideStep Trigger
        {
            if(Random.Range(0, 100.1f) < sideStepChance)
            {
                print("0");
                SideStepCheck();
            }
        }
    }
    private void Update()
    {
<<<<<<< Updated upstream
        if(Input.GetKeyDown(KeyCode.K))
        {
            //Destroy(gameObject);
            return;
        }
        playerPoint = CanSeePlayer();
        if(playerPoint)
=======
        // if(Input.GetKeyDown(KeyCode.K))
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        playerPoint = GetSeenPlayerPoint();
        canSeePlayer = playerPoint;
        if(canSeePlayer)
>>>>>>> Stashed changes
        {
            if(!playerInSightTrigger)
            {
                playerInSightTrigger = true;
                agent.updateRotation = false;
            }
           if(currentState != AIStates.Aggro)
            {
                SwitchStates(AIStates.Aggro);
            }
        }
        else if(playerInSightTrigger)
        {
            playerInSightTrigger = false;
            agent.updateRotation = true;
            lastSeenPlayerPosition = playerTransform.position;
        }
        if(currentState == AIStates.Aggro)
        {
            RotateTowardsTarget();
        }
<<<<<<< Updated upstream
=======
        if(agent.isStopped && rigAnimator.GetBool("Walking"))
        {
            rigAnimator.SetBool("Walking", false);
        }
        else if(!agent.isStopped && !rigAnimator.GetBool("Walking"))
        {
            rigAnimator.SetBool("Walking", true);
        }
>>>>>>> Stashed changes
    }

    IEnumerator Aggro()
    {
        bool checkedForCover = false;
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
            if (!inCover && !checkedForCover)
            {
                checkedForCover = true;
                Vector3 point = GetCoverPointOnNavMesh(FindClosestCover());
                print(point);
                if(point != Vector3.zero)
                {
                    if(Vector3.Distance(transform.position, point) > Vector3.Distance(transform.position, playerDamageHandler.transform.position))

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
    void RotateTowardsTarget()
    {
        Vector3 lookDir = playerTransform.position;
        lookDir.y = transform.position.y;
        lookDir = lookDir - transform.position;
        // transform.LookAt(lookDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), maxSnapRotateSpeed * Time.deltaTime);
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
    void SideStepCheck()
    {
        if(sideStepping)
        {
            print("SideStepBug");
            return;
        }
        int direction;
        direction = Random.Range(0, 2) == 0 ? -1 : 1; //returns -1 or 1
        if(SideStepRayCastCheck(direction))
        {
            print("2");
            NavMeshHit hit;  
            if(!NavMesh.SamplePosition(transform.right * direction + transform.position, out hit, 1, NavMesh.AllAreas))
            {
                print("3");
                return;
            }
            StartCoroutine(SideStep(hit, direction));
        }
    }
    bool SideStepRayCastCheck(int dir)
    {
        if(Physics.Raycast(transform.position, transform.right * dir, sideStepMaxDistance))
        {
            if(Physics.Raycast(transform.position, transform.right * -dir, sideStepMaxDistance))
                return false;
        }
        return true;
        
    }
    IEnumerator SideStep(NavMeshHit hit, int dir)
    {
       print("SideStepping");
        sideStepping = true;
        
        if(dir == 1)
            rigAnimator.SetTrigger("StepRight");
        else
            rigAnimator.SetTrigger("StepLeft");
        float clipLength = rigAnimator.GetCurrentAnimatorStateInfo(1).length;
        float _time = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = hit.position; 
        canSideStep = false;
        while(_time < 1)
        {
            yield return null;
            _time += Time.deltaTime / clipLength;
            transform.position = Vector3.Lerp(startPos, endPos, _time);
        }
        sideStepping = false;
        yield return Timer(sideStepCoolDownTime);
        canSideStep = true;
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
