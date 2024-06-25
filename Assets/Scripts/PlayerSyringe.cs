using UnityEngine;
using System.Collections;

public class PlayerSyringe : MonoBehaviour
{
    [SerializeField] float amountOfSyringes;
    [SerializeField] float healAmount;
    [SerializeField] float healTime = 0.5f;
    [SerializeField] float delayToHeal;

    PlayerDamageHandler damageHandler;

    bool healing;
    [SerializeField]Animation animator;

    static public PlayerSyringe instance;
    private void Start()
    {
        instance = this;
        damageHandler = GetComponent<PlayerDamageHandler>();
    }
    void Update()
    {
        if (healing || amountOfSyringes <= 0)
            return;

        if(Input.GetKeyDown(KeyCode.H))
        {
            healing = true;
            BaseGun.adsForbidden = true;
            animator.Play();
            StartCoroutine("Heal");
            
        }
    }

    IEnumerator Heal()
    {
        float timer = healTime;
        yield return new WaitForSeconds(delayToHeal);
        BaseGun.adsForbidden = false;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            damageHandler.Heal(healAmount * (Time.deltaTime / healTime));  
            yield return null;
        }

        amountOfSyringes--;
        healing = false;
        
    }

    public void GainSyringe(float count)
    {
        amountOfSyringes += count;
    }


}
