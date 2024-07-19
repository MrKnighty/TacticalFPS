using UnityEngine;

public class EventSecurityOverride : Interactable
{
    bool ready = false;
    [SerializeField] GameObject[] Gates;
    [SerializeField] float lightsOutTime;
    [SerializeField] GameObject[] newEnemiesToEnable;
    Light[] lights;
    Animation anim;

    protected override void Start()
    {
        lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
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

    void LightsOut()
    {
        foreach(Light light in lights)
        {
            if (light.gameObject.isStatic)
                light.enabled = false;
        }
        foreach(GameObject enemy in newEnemiesToEnable)
        {
            enemy.SetActive(true);
        }

        UnityEngine.Rendering.ProbeReferenceVolume.instance.lightingScenario = "lights_out";
    
    }

}
