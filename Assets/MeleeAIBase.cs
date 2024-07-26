using System.Collections;
using UnityEngine;

public class MeleeAIBase : AIBase
{
    Transform playerPoint;
    bool aggro;
    [Header("Audio")]
    [SerializeField]AudioClip[] idleAudioClips;
    [SerializeField]AudioClip[] aggroAudioClips;
    [SerializeField]float timeBetweenIdleClips = 0;
    [Header("MeleeAI")]
    [SerializeField] float attackCooldown = 1;
    [SerializeField] float attackDistance = 2;
    [SerializeField] float attackDamage = 20;
    bool canAttack = true;
    
    Animator animator;
    int currentIdleSound = 0;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        currentIdleSound = Random.Range(0, idleAudioClips.Length);
        if(!aggro)
            StartCoroutine(IdleSounds());
    }

    IEnumerator IdleSounds()
    {
        while(true)
        {
            currentIdleSound ++;
            if(currentIdleSound == idleAudioClips.Length)
                currentIdleSound = 0;
            audioSource.PlayOneShot(idleAudioClips[currentIdleSound]);
            yield return new WaitForSeconds(idleAudioClips[currentIdleSound].length + timeBetweenIdleClips);
        }
    }
    private void Update() 
    {
        playerPoint = CanSeePlayer();
        if(!aggro)
        {
            if(playerPoint)
            {
                aggro = true;
                animator.SetBool("Moving", true);
                StopAllCoroutines();
                audioSource.Stop();
                audioSource.PlayOneShot(aggroAudioClips[Random.Range(0, aggroAudioClips.Length)]); //Play Audio Clip Randomly from Array
            }
            else
            return;
        }
        Chase();
    }

    void Chase()
    {
        if(canAttack)
            if(DistanceCheck())
            {
                animator.SetTrigger("Attack");
                canAttack = false;
            }
        Vector3 dir = transform.position - playerTransform.position;
        dir = dir.normalized;
        Vector3 point = playerTransform.position + dir * (attackDistance - 0.1f);
        agent.SetDestination(point);
    }
    [ContextMenu("AnimatorAttack")]
    void AnimatorAttack()
    {
        playerDamageHandler.Damage(20);
        StartCoroutine(AttackCoolDown());
    }
    IEnumerator AttackCoolDown()
    {
        yield return Timer(attackCooldown);
        canAttack = true;
    }
    public override void DamageTrigger()
    {
        aggro = true;
        Vector3 pos = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.LookAt(pos);
        animator.SetBool("Moving", true);
    }
    

    bool DistanceCheck()
    {
        if(Vector3.Distance(transform.position, playerTransform.position) < attackDistance)
            return true;

        return false;
    }
}
