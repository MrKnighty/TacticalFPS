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

    [SerializeField] AudioSource gunshotSource;
    [SerializeField] AudioClip[] gunshotSounds;
   // Light[] lights;
    Animation anim;

    ReflectionProbe[] probes;

    protected override void Start()
    {
      //  lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        base.Start();
        anim = GetComponent<Animation>();

        probes = FindObjectsOfType<ReflectionProbe>();

       

    }
    protected override void TriggerEvent()
    {
        ready = true;
        foreach(GameObject gate in Gates)
        {
            gate.GetComponent<AudioSource>().Play();
        }
        anim.Play();

        LightsOut();
    }

    private void OnTriggerEnter(Collider other)
    {
      
        
    }
    [ContextMenu("LightsOut")]
    void LightsOut()
    {

      


        StartCoroutine("LightsFlicker");

       
            foreach (GameObject gate in Gates)
            {
                gate.GetComponent<Animation>().Play();
                print(gate.name);
            }

         

        
    }
    LightmapData[] data;
    IEnumerator LightsFlicker()
    {
        data = LightmapSettings.lightmaps;
        yield return new WaitForSeconds(lightsOutTime);
        UnityEngine.Rendering.ProbeReferenceVolume.instance.lightingScenario = "lights_out";
        foreach (ReflectionProbe probe in probes)
        {
            probe.enabled = false;
        }
        foreach (Light light in lights)
        {

            light.gameObject.SetActive(false);
            LightmapSettings.lightmaps = new LightmapData[]{};
        }

        for (int i =0; i< 8; i++)
        {
            UnityEngine.Rendering.ProbeReferenceVolume.instance.lightingScenario = "lights_on";
            foreach (Light light in lights)
            {

                light.gameObject.SetActive(true);
                LightmapSettings.lightmaps = data;
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            UnityEngine.Rendering.ProbeReferenceVolume.instance.lightingScenario = "lights_out";
            foreach (Light light in lights)
            {

                light.gameObject.SetActive(false);
                LightmapSettings.lightmaps = new LightmapData[] { };
            }
            yield return new WaitForSeconds(Random.Range(0.1f,0.2f));

        }
       

       

        for(int i = 0; i < 20; i++)
        {
            gunshotSource.PlayOneShot(gunshotSounds[Random.Range(0, gunshotSounds.Length -1)],GameControllsManager.audioVolume );
            yield return new WaitForSeconds(Random.Range(0.1f,0.25f));
        }
        foreach (GameObject enemy in newEnemiesToEnable)
        {
            enemy.SetActive(true);
        }
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(true);
        }
    }

}
