using UnityEngine;

public enum BodyParts {Head, Body, Legs, Arms}
public class DamageHandler : MonoBehaviour
{
    [SerializeField] float maxHealth;
    public float currentHealth;
    [SerializeField] bool dropItem;
    [SerializeField] GameObject itemToDrop;

    [SerializeField, Tooltip("Head, Body, Legs, Arms, calculation: damage = damageAmount * bodyPartMultiplier * bodyPartArmour")] 
    float[] bodyPartArmour = {1,1,1,1};
    [SerializeField, Tooltip("Head, Body, Legs, Arms")]
    float[] bodyPartMultiplier = {2,1,0.5f,0.5f};
   
    [SerializeField] GameObject ragdollObject;
    [SerializeField] bool useRagDoll = true;
    AIBase aiBase;



    protected void Start()
    {
        aiBase = GetComponent<AIBase>();
        currentHealth = maxHealth;
    }
    public void Damage(float damageAmount, BodyParts bodyPart = BodyParts.Body)
    {
        currentHealth -= damageAmount * bodyPartMultiplier[(int)bodyPart] * bodyPartArmour[(int)bodyPart];
        if (aiBase)
        {
            aiBase.DamageTrigger();
        }

        if (currentHealth <= 0)
            DeathEvent();
    }
    protected void DeathEvent()
    {
        Destroy(gameObject);
        if(useRagDoll)
        {
            Instantiate(ragdollObject, transform.position, transform.rotation).GetComponent<Rigidbody>().AddForce(10,10,10);
        }
        if(dropItem)
        {
            Instantiate(itemToDrop, transform.position, transform.rotation);
        }
    }
    
}
