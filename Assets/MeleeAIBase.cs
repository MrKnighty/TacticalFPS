using System.Collections;
using UnityEngine;

public class MeleeAIBase : AIBase
{
    Transform playerPoint;
    bool aggro;
    [Header("MeleeAI")]
    [SerializeField] float attackCooldown = 1;
  
    [SerializeField] float attackDistance = 2;
    [SerializeField] float attackDamage = 20;
    bool canAttack = true;
    
    Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
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
        agent.SetDestination(playerTransform.position);
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
        animator.SetBool("Moving", true);
    }
    

    bool DistanceCheck()
    {
        if(Vector3.Distance(transform.position, playerTransform.position) < attackDistance)
            return true;

        return false;
    }
}
