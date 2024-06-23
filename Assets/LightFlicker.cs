using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] float offTimeMin, offTimeMax;
    [SerializeField] Light light;


    private void Start()
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {

        while(true)
        {
            light.enabled = false;
            yield return new WaitForSeconds(Random.Range(offTimeMin, offTimeMax));
            light.enabled = true;
            yield return new WaitForSeconds(Random.Range(offTimeMin, offTimeMax));

            
        }
      
    }


}
