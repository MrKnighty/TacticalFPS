using UnityEngine;

public class MaterialPropertiesManager : MonoBehaviour
{
    [System.Serializable] public struct MaterialProperty{ public Material[] material;public AudioClip[] stepSounds, shotSounds; public GameObject hitParticle, decal;}


    [SerializeField] MaterialProperty[] materialProperties;
    [SerializeField] MaterialProperty fallbackMat;

    static MaterialProperty[] sMaterialProperties;
    static MaterialProperty sFallbackMat;
    void Start()
    {
        sMaterialProperties = materialProperties;
        sFallbackMat = fallbackMat;
    }

    public static AudioClip[] GetFootStepSounds(GameObject hitObject)
    {
        int i = GetIndex(hitObject);
   
        if (i == -1 || sMaterialProperties[i].stepSounds == null)
           return sFallbackMat.stepSounds;
        return sMaterialProperties[i].stepSounds; 
    }
    public static AudioClip[] GetBulletImpactSounds(GameObject hitObject)
    {
        int i = GetIndex(hitObject);
        print(i + " " + sMaterialProperties[i].shotSounds == null);
        if (i == -1 || sMaterialProperties[i].shotSounds == null)
           return sFallbackMat.shotSounds;
        return sMaterialProperties[i].shotSounds; 
    }

    public static GameObject GetHitParticle(GameObject hitObject)
    {
        int i = GetIndex(hitObject);
        if (i == -1 || sMaterialProperties[i].hitParticle == null)
           return sFallbackMat.hitParticle;
        return sMaterialProperties[i].hitParticle; 
    }

    public static GameObject GetDecal(GameObject hitObject)
    {
        int i = GetIndex(hitObject);
        if (i == -1 || sMaterialProperties[i].decal == null)
            return sFallbackMat.decal;
          
        return sMaterialProperties[i].decal; 
    }


    static int GetIndex(GameObject hitObject)
    {
          Material mat = null;
        if(hitObject.transform.GetComponent<MeshRenderer>())
           mat = hitObject.transform.GetComponent<MeshRenderer>().material;   
        else if(hitObject.transform.GetComponentInChildren<MeshRenderer>())
            mat = hitObject.transform.GetComponentInChildren<MeshRenderer>().material; 
        else
        {
            mat = null;
            print("No material found on root object or child!");
            return -1;
            
        }
        DebugManager.DisplayInfo("LHOBJ", "Texture: " + mat.ToString());
        
         int i = 0;

        foreach (MaterialProperty prop in sMaterialProperties) 
        {
            foreach(Material material in prop.material)
            {
                if (material.name + " (Instance)" == mat.name) // for some reason the material we dynamically get has (instance) at the end of it, making this comparison impossible without this bodge, findd a better way me!
                    return i;
            }
       
            i++;
        }

        DebugManager.DisplayInfo("HOBJERR", "Hit Object does not have material properties set! " + mat.ToString());
        return -1;
    
    }

    
}
