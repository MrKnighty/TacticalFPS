using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float speed;
    [SerializeField] bool autoStart;


    private void Start()
    {
        if(autoStart)
         StartCoroutine("Elevate");
    }
   
  
    IEnumerator Elevate()
    { 
        while(Vector3.Distance(transform.position, target.transform.position) > 0.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        yield return null;
    }
    [ContextMenu("StartElevator")]
    public void StartElevator()
    {
        StartCoroutine("Elevate");
    }
    
}
