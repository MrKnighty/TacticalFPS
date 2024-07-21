using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EventSecurityOverride : Interactable
{
    bool ready = false;
    [SerializeField] GameObject[] Gates;
    [SerializeField] float lightsOutTime;
    [SerializeField] GameObject[] newEnemiesToEnable;
    [SerializeField] GameObject[] objectsToEnable;
    [SerializeField] Light[] lights;
   // Light[] lights;
    Animation anim;

    protected override void Start()
    {
      //  lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        base.Start();
        anim = GetComponent<Animation>();

    }
    protected override void TriggerEvent()
    {
        ready = true;
        foreach(GameObject gate in Gates)
        {
            gate.GetComponent<AudioSource>().Play();
        }
        anim.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player" && ready)
        {
            foreach (GameObject gate in Gates)
            {
                gate.GetComponent<Animation>().Play();
                print(gate.name);
            }

            Invoke("LightsOut", lightsOutTime);
        }
    }
    [ContextMenu("LightsOut")]
    void LightsOut()
    {
       
        foreach(GameObject enemy in newEnemiesToEnable)
        {
            enemy.SetActive(true);
        }
        foreach(GameObject obj in objectsToEnable)
        {
            obj.SetActive(true);
        }
        StartCoroutine("LightsFlicker");

        
    
    }

    IEnumerator LightsFlicker()
    {
        UnityEngine.Rendering.ProbeReferenceVolume.instance.lightingScenario = "lights_out";
        foreach (Light light in lights)
        {

            light.enabled = false;
        }
        yield return new WaitForSeconds(0.5f);

        UnityEngine.Rendering.ProbeReferenceVolume.instance.lightingScenario = "lights_on";
        foreach (Light light in lights)
        {

            light.enabled = true;
        }

        yield return new WaitForSeconds(0.2f);

        UnityEngine.Rendering.ProbeReferenceVolume.instance.lightingScenario = "lights_out";
        foreach (Light light in lights)
        {

            light.enabled = false;
        }

        yield return new WaitForSeconds(0.5f);

        UnityEngine.Rendering.ProbeReferenceVolume.instance.lightingScenario = "lights_on";
        foreach (Light light in lights)
        {

            light.enabled = true;
        }

        yield return new WaitForSeconds(0.5f);

        UnityEngine.Rendering.ProbeReferenceVolume.instance.lightingScenario = "lights_out";
        foreach (Light light in lights)
        {

            light.enabled = false;
        }
    }

}
