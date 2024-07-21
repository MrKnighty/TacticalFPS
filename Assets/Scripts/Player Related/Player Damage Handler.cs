using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDamageHandler : MonoBehaviour
{
    [SerializeField] float maxHealth;
    public float currentHealth;

    [SerializeField] GameObject DeathScreenUIGameObject;
    [SerializeField] bool loadSceneOnDeath = false;

    [SerializeField] AudioClip[] hurtAudioClips;
    [SerializeField] AudioClip[] healAudioClips;

    AudioSource audioSource;

    [SerializeField] RawImage damageImage;
    [SerializeField] float damageImageFadeSpeed;
    [SerializeField] bool godMode = false;
    [SerializeField] AudioClip godModeAudioClip;
    [SerializeField] Image hpImage;
    void Start()
    {
        try
        {
            audioSource = GetComponent<AudioSource>();
        }
        catch 
        {
            print("No Audio Source On Player: " + this.name);
        }

    

    }

    public void LoadPersistantData()
    {
        currentHealth = PresistantPlayerData.health;
        UICommunicator.UpdateUI("Hp Text", "" + (int)currentHealth);
        hpImage.fillAmount = currentHealth / maxHealth;
    }

    public void WritePersistantData()
    {
        PresistantPlayerData.health = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UICommunicator.UpdateUI("Hp Text", "" + (int)currentHealth);
        if (Input.GetKeyDown(KeyCode.P))
        {
            godMode = !godMode;
            if (godMode)
            {
                audioSource.PlayOneShot(godModeAudioClip);
                UICommunicator.refrence.PopupText("Phuong Mode Actiavted", 2f);
            }
            else
            {
                UICommunicator.refrence.PopupText("Phuong Mode Deactiavted", 2f);
            }

        }
    }
    public void Heal(float amount)
    {
        ChangeHealth(amount);
        if (audioSource)
            try {audioSource.PlayOneShot(healAudioClips[UnityEngine.Random.Range(0, healAudioClips.Length)]); }
            catch { print("No Heal Audio Clips in Array: " + this.name); }
            
    }
    public void Damage(float amount)
    {
        if (godMode)
            return;
        ChangeHealth(-amount);
        if (audioSource)
            try {audioSource.PlayOneShot(hurtAudioClips[UnityEngine.Random.Range(0, hurtAudioClips.Length - 1)]); }
            catch { print("No Hurt Audio Clips in Array: " + this.name); }
        if(damageImage != null)
        {
            StopCoroutine(DamageImageFade());
            StartCoroutine(DamageImageFade());
        }
    }
    IEnumerator DamageImageFade()
    {
        float alpha = 1;
        Color color = damageImage.color;
        while(alpha > 0)
        {
            alpha -= damageImageFadeSpeed * Time.deltaTime;
            color.a = alpha;
            damageImage.color = color;
            yield return null;
        }
    }

    void ChangeHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UICommunicator.UpdateUI("Hp Text", "" + (int)currentHealth);
        hpImage.fillAmount = currentHealth / maxHealth; 
        if (currentHealth <= 0) 
        {
            Death();
        }
    }
    void Death()
    {
        /* if(loadSceneOnDeath)
         {
             SceneManager.LoadScene(0);
             return;
         }
         if(DeathScreenUIGameObject)
         {
             DeathScreenUIGameObject.SetActive(true);
         }
         else
         {
             print("No Death Screen UI Assigned");
         }*/

        UICommunicator.refrence.GameOverUI();
    }
   
}
