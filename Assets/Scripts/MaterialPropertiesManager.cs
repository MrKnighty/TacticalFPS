using UnityEngine;

public class MaterialPropertiesManager : MonoBehaviour
{
    [System.Serializable] public struct MaterialProperty{public Material material; public AudioClip[] stepSounds, shotSounds; public GameObject hitParticle, decal;}


    [SerializeField] MaterialProperty[] materialProperties;
    [SerializeField] MaterialProperty fallbackMat;

    static MaterialProperty[] sMaterialProperties;
    static MaterialProperty sFallbackMat;
    void Start()
    {
        sMaterialProperties = materialProperties;
    }

    public static AudioClip[] GetFootStepSounds(GameObject hitObject)
    {
        int i = GetIndex(hitObject);
        if (i == -1)
           return sFallbackMat.stepSounds;
        return sMaterialProperties[i].stepSounds; 
    }
    public static AudioClip[] GetBulletImpactSounds(GameObject hitObject)
    {
   int i = GetIndex(hitObject);
        if (i == -1)
           return sFallbackMat.shotSounds;
        return sMaterialProperties[i].shotSounds; 
    }

    public static GameObject GetHitParticle(GameObject hitObject)
    {
        int i = GetIndex(hitObject);
        if (i == -1)
           return sFallbackMat.hitParticle;
        return sMaterialProperties[i].hitParticle; 
    }

    public static GameObject GetDecal(GameObject hitObject)
    {
        int i = GetIndex(hitObject);
        if (i == -1)
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
        }
        if (mat != null)
             DebugManager.DisplayInfo("LHOBJ", "Texture: " + mat.ToString());
        
         int i = 0;

        foreach (MaterialProperty prop in sMaterialProperties) 
        {
            if(prop.material == mat)
                return i;   
        }

        DebugManager.DisplayInfo("HOBJERR", "Hit Object does not have material properties set! " + mat.ToString());
        return -1;
    
    }

    
}
