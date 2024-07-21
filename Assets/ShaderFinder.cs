using UnityEngine;
using System.Collections.Generic;

public class ShaderFinder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.LogError("SHADER FINDER IS RUNNING! ARE YOU SURE YOU WANT ME ON");
    }
    [SerializeField] GameObject[] everything;

    List<Material> allFoundMats = new();
    // Update is called once per frame
    void Update()
    {
      

        foreach(GameObject m in everything)
        {
            if (!m.TryGetComponent<Renderer>(out Renderer reed))
                continue;
            if (allFoundMats.Contains(reed.material))
                continue;

            allFoundMats.Add(reed.material);
        }    
    }
    [SerializeField] GameObject cube;
    [ContextMenu("SpawnCubes")]
    void SpawnCubes()
    {
        foreach (Material m in allFoundMats)
        {
            GameObject c = Instantiate(cube);
            c.GetComponent<Renderer>().material = m;
        }
    }
}
