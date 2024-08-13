using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DropMag : MonoBehaviour
{
    
    [SerializeField] GameObject magazine;
    [SerializeField]Transform dropPoint;
    List<GameObject> magazinePool;
    [SerializeField] float magazineLifeTime;
    public void SpawnMagazine()
    {
        if(magazinePool.Count > 0)
        {
            ResetMag(magazinePool[0]);
            magazinePool[0].SetActive(true);
            StartCoroutine(Timer(magazinePool[0]));
        }
        else
        {
            StartCoroutine(Timer(Instantiate(magazine, dropPoint.position, quaternion.identity)));
        }
    }
    
    IEnumerator Timer(GameObject mag)
    {
        float t = 0;
        while( t < magazineLifeTime)
        {
            if(UICommunicator.gamePaused)
                yield return null;
            yield return null;
            t += Time.deltaTime;
        }
        yield return null;
        mag.SetActive(false);
        magazinePool.Add(mag);
    }
    void ResetMag(GameObject mag)
    {
        mag.transform.position = dropPoint.position;
        mag.transform.rotation = Quaternion.identity;
    }

    


}
