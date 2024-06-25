using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDamageHandler : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;

    [SerializeField] GameObject DeathScreenUIGameObject;
    [SerializeField] bool loadSceneOnDeath = false;

    [SerializeField] AudioClip[] hurtAudioClips;
    [SerializeField] AudioClip[] healAudioClips;

    AudioSource audioSource;

    [SerializeField]Image damageImage;
    [SerializeField] float damageImageFadeSpeed;

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

    // Update is called once per frame
    void Update()
    {

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
        ChangeHealth(-amount);
        if (audioSource)
            try {audioSource.PlayOneShot(healAudioClips[UnityEngine.Random.Range(0, hurtAudioClips.Length)]); }
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
        UICommunicator.UpdateUI("Hp Text", "HP: " + (int)currentHealth);
        if(currentHealth <= 0) 
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
