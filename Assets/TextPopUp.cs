using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading;

public class TextPopUp : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] TextMeshProUGUI textMeshPro;
    [SerializeField] string text;
    [SerializeField] float fadeInTime = 0;
    [SerializeField] float fadeOutTime = 0;
    [SerializeField] bool onExit;
    [SerializeField] float duration = 5;
    [SerializeField] bool destroy;

    bool trigger = false;
    bool exitTrigger = false;
    private void Start() {
        //GetComponent<BoxCollider>().enabled = false;
        //GetComponent<BoxCollider>().enabled = true;

    }
    private void OnTriggerEnter(Collider other) 
    {
        if(trigger)
            return;

        if(other.gameObject.tag == "Player")
        {
            trigger = true;
            Color c = textMeshPro.color;
            Color colorNoAlpha = new Color (c.r, c.g, c.b, 0);
            Color colorAlpha = new Color (c.r, c.g, c.b, 1);

            StartCoroutine(FadeBetweenText(colorNoAlpha, colorAlpha, fadeInTime, true));
            textMeshPro.gameObject.SetActive(true);
        }
        StartCoroutine(Timer());
    }
    
    IEnumerator Timer()
    {
        float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            yield return null;
        }
        Color c = textMeshPro.color;
        Color colorNoAlpha = new Color (c.r, c.g, c.b, 0);
        Color colorAlpha = new Color (c.r, c.g, c.b, 1);
        StartCoroutine(FadeBetweenText(colorAlpha, colorNoAlpha, fadeOutTime, false, destroy));
    }
    private void OnTriggerExit(Collider other) 
    {
        if(exitTrigger || !onExit)
            return;
            
        if(other.gameObject.tag == "Player")
        {
            exitTrigger = true;
            Color c = textMeshPro.color;
            Color colorNoAlpha = new Color (c.r, c.g, c.b, 0);
            Color colorAlpha = new Color (c.r, c.g, c.b, 1);
            StartCoroutine(FadeBetweenText(colorAlpha, colorNoAlpha, fadeOutTime, false, destroy));

        }
    }

    IEnumerator FadeBetweenText(Color a, Color b, float fadeTime, bool state, bool destroyObject = false)
    {
        textMeshPro.text = text;
        
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / fadeTime;
            textMeshPro.color = Color.Lerp(a, b, timer);
            yield return null;
        }
        textMeshPro.gameObject.SetActive(state);
        if(destroyObject)
        Destroy(gameObject);
    }

}
