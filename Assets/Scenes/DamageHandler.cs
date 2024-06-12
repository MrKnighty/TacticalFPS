using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] GameObject ragdollObject;

    protected float currentHealth;

    protected void Start()
    {
        currentHealth = maxHealth;
    }
    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth < 0)
            DeathEvent();
    }
    protected void DeathEvent()
    {
        Instantiate(ragdollObject, transform.position, transform.rotation).GetComponent<Rigidbody>().AddForce(10,10,10);
        Destroy(gameObject);
    }
    
}
