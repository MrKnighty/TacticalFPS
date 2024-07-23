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
    [SerializeField] Color lowHPColor;
    [SerializeField] float hpPingPongSpeed = 2;
    [SerializeField] float hpImagePingPongSize = 1;
    Color hpColor;
    Vector3 hpImageScale;
    void Start()
    {
        hpImageScale = hpImage.transform.localScale;
        hpColor = hpImage.color;
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
    float pingPongTime = 0;
    bool lowHpTrigger = false;
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
        if(currentHealth < 40)
        {
            pingPongTime += Time.deltaTime * hpPingPongSpeed;
            hpImage.color = Color.Lerp(hpColor, lowHPColor, Mathf.PingPong(pingPongTime, 1));
            hpImage.transform.localScale = Vector3.Lerp(hpImageScale, hpImageScale * hpImagePingPongSize, pingPongTime);
            if(!lowHpTrigger)
            {lowHpTrigger = true;}
        }
        else if(lowHpTrigger)
        {
            lowHpTrigger = false;
            pingPongTime = 0;
            hpImage.color = hpColor;
            hpImage.transform.localScale = hpImageScale;

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
