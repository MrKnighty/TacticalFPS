using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicShootAI : AIBase
{
    [Header("Melee")]
    [SerializeField] float meleeCooldownTime = 2f;
    [SerializeField] int meleeDamage = 10;
    [SerializeField] float meleeDistance = 2;
    bool canMelee = true;
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
    [SerializeField] Animator rigAnimator;
    
    //Triggers
    bool playerInSightTrigger;
    protected override void Start()
    {
        base.Start();
        SwitchStates(currentState);
    }
    void ExitState()
    {
        rigAnimator.SetBool("Walking", false);
        rigAnimator.SetBool("Aggro", false);
        switch (currentState)
        {
            case AIStates.Aggro:
            break;
            
        }
    }
    void SwitchStates(AIStates state)
    {
        StopAllCoroutines();
        agent.isStopped = true;
        ExitState();
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
                UICommunicator.refrence.PopupText("Switch>Aggro", 2f);

                currentState = state;
                rigAnimator.SetBool("Aggro", true);
                StartCoroutine(Aggro());
                StartCoroutine(Shoot());
                break;
            case AIStates.Chase:
                currentState = state;

                StartCoroutine(Chase());
                break;
            case AIStates.Melee:
                UICommunicator.refrence.PopupText("Switch>Melee", 2f);
                StartCoroutine(MeleeAttack());
            break;
        }

    }
    IEnumerator MeleeAttack()
    {
        canMelee = false; 
        rigAnimator.SetTrigger("Melee");
        yield return Timer(0.6f);
        yield return Relocate(false);
        SwitchStates(AIStates.Aggro);
        yield return Timer(meleeCooldownTime);
        canMelee = true; 
    }
    public override void DamageTrigger()
    {
        if(!inCover && canSideStep) //SideStep Trigger
        {
            if(Random.Range(0, 100.1f) < sideStepChance)
            {
                SideStepCheck();
            }
        }
        if(currentState != AIStates.Aggro)
        {
            SwitchStates(AIStates.Aggro);
        }
    }
    float frames = 0;
    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.K))
        // {
        //     Destroy(gameObject);
        //     return;
        // }
       
        playerPoint = GetSeenPlayerPoint();
        canSeePlayer = playerPoint;
        if(canSeePlayer)
        {
            if(!playerInSightTrigger)
            {
                playerInSightTrigger = true;
                agent.updateRotation = false;
            }
           if(currentState != AIStates.Aggro && currentState != AIStates.Melee)
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
        if(agent.isStopped && rigAnimator.GetBool("Walking"))
        {
            rigAnimator.SetBool("Walking", false);
        }
        else if(!agent.isStopped && !rigAnimator.GetBool("Walking"))
        {
            rigAnimator.SetBool("Walking", true);
        }
        if(frames > 15 && currentState == AIStates.Aggro)//Stop distance check from lagging
        {
            frames = 0;
            if(!canMelee)
                return;
            if(Vector3.Distance(transform.position, playerTransform.position) < meleeDistance)
            {
                print("Switching to melee");
                SwitchStates(AIStates.Melee);
            }
        }
        else
        {
            frames ++;
        }
    }

    IEnumerator Aggro()
    {
        if(!playerPoint)
            playerPoint = playerBodyPoint;
        if (!inCover)
        {
            Vector3 point = GetCoverPointOnNavMesh(FindClosestCover());
            if(point != Vector3.zero)
            {
                if(Vector3.Distance(transform.position, point) > Vector3.Distance(transform.position, playerDamageHandler.transform.position))

                yield return MoveToDestination(point);
                inCover = true;
            }
            else 
            {
                StartCoroutine(Relocate(true));
            }
        }
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
            
            yield return null;
        }
    }
    IEnumerator Relocate(bool dir) //Move Slightly Towards / away from player
    {
        UICommunicator.refrence.PopupText("Relocating", 2f);
        Vector3 point = GetNavmeshPointInRadiusTowardsPlayer(movemnetRadius, dir);
        if(point != Vector3.zero)
        {
            yield return MoveToDestination(point);
            rigAnimator.SetBool("Crouch", true);
            isCrouched = true;
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
                yield return StartCoroutine(Reload());
            }
            if (!playerPoint)
            {
                lostSight = true;
                yield return null;
                continue;
            }
            if(ShootAtTarget(playerPoint.position, shootRadius))
            {
                UICommunicator.refrence.PopupText("ShootingPlayer", 0.1f);

                playerDamageHandler.Damage(damage);
            }
            if (muzzleFX)
                muzzleFX.Play(true);
            ammo--;
            yield return Timer(attackSpeed);
        }

    }
    IEnumerator Reload()
    {
        UICommunicator.refrence.PopupText("Reloading", 2f);
        rigAnimator.SetTrigger("Reload");
        if(damageHandler.currentHealth > damageHandler.maxHealth * 0.5f) // HP is greater than half == high confidence
        {
            StartCoroutine(Relocate(true)); //advnace towards player
        }
        else
        {
            Vector3 destination;
            if((destination = GetCoverPointOnNavMesh(FindClosestCover())) != Vector3.zero) // Go to cover
            {
                MoveToDestination(destination);
                UICommunicator.refrence.PopupText("Moving To Cover", 2f);

            }
            else
            {
                StartCoroutine(Relocate(false));//Retreat away from player
            }
        }
        rigAnimator.SetTrigger("Reload");
        yield return Timer(reloadTime);
         UICommunicator.refrence.PopupText("FinReloading", 2f);
        ammo = ammoCap;
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
        
        if(sideStepping || !agent.isStopped || currentState != AIStates.Aggro)
        {
            return;
        }
        int direction;
        direction = Random.Range(0, 2) == 0 ? -1 : 1; //returns -1 or 1
        if(SideStepRayCastCheck(direction))
        {
            NavMeshHit hit;  
            if(!NavMesh.SamplePosition(transform.right * direction + transform.position, out hit, 1, NavMesh.AllAreas))
            {
                return;
            }
            if(isCrouched == true)
            {
                UnCrouch();
            }
            StartCoroutine(SideStep(hit, direction));
        }
    }
    void Crouch()
    {
        isCrouched = true;
        rigAnimator.SetBool("Crouch", true);
    }
    void UnCrouch()
    {
        isCrouched = false;
        rigAnimator.SetBool("Crouch", false);
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
        yield return null;
        while(rigAnimator.GetAnimatorTransitionInfo(1).duration > 0)
        {
            print("x");
            yield return null;
        }
        float clipLength = rigAnimator.GetCurrentAnimatorStateInfo(1).length / 2;
        Vector3 startPos = transform.position;
        Vector3 endPos = hit.position; 
        canSideStep = false;
        float elapsedTime = 0f;
        print(clipLength);
        while(elapsedTime < clipLength)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / clipLength);
            transform.position = Vector3.Lerp(startPos, endPos, t);
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
        if(isCrouched)
            UnCrouch();
        rigAnimator.SetBool("Walking", true);
        agent.isStopped = false;
        agent.destination = point;
        while (Vector3.Distance(transform.position, point) > 0.2f)
        {
            yield return null;
        }
        agent.isStopped = true;
        rigAnimator.SetBool("Walking", false);
        
    }

}
