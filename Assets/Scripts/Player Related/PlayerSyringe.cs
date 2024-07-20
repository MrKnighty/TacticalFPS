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
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip healStartSound, healingSound, healEndSound;

    static public PlayerSyringe instance;
    private void Start()
    {
        instance = this;
        damageHandler = GetComponent<PlayerDamageHandler>();
      
    }
    void Update()
    {
        UICommunicator.UpdateUI("Syringe Text", "" + amountOfSyringes);

        if (healing || amountOfSyringes <= 0|| damageHandler.currentHealth >= 100)
            return;

        if(Input.GetKeyDown(KeyCode.H))
        {
            healing = true;
            BaseGun.adsForbidden = true;
            animator.Play();
            StartCoroutine("Heal");
            
        }

        UICommunicator.UpdateUI("Syringe Text", "" + amountOfSyringes);
    }

    IEnumerator Heal()
    {
        float timer = healTime;
        yield return new WaitForSeconds(delayToHeal);
        source.PlayOneShot(healStartSound);
        BaseGun.adsForbidden = false;
        amountOfSyringes--;
        source.PlayOneShot(healingSound);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            damageHandler.Heal(healAmount * (Time.deltaTime / healTime));  
            yield return null;
        }

        source.PlayOneShot(healEndSound);
        healing = false;
        
    }

    public void GainSyringe(float count)
    {
        amountOfSyringes += count;
        UICommunicator.UpdateUI("Syringe Text", "Syringe: " + amountOfSyringes);
    }


}
