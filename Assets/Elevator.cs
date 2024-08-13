using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float speed;
    [SerializeField] bool autoStart;

  

    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] float timeToClose;
    [SerializeField] GameObject playerRefrence;
    float currentTime;


    private void Start()
    {
        if(autoStart)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(0,0);
            StartCoroutine("Elevate");
        }
             
    }

    IEnumerator CloseDoorAnim()
    {
        currentTime = timeToClose;
        skinnedMeshRenderer.SetBlendShapeWeight(0, 1);
        while (currentTime >= 0)
        {
            currentTime -= Time.deltaTime;
        
            skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Lerp(0, 100, currentTime / timeToClose));
            yield return null;
        }
        yield return null;
    }

    IEnumerator OpenDoorAnim()
    {
        currentTime = timeToClose;
        while (currentTime >= 0)
        {
            currentTime -= Time.deltaTime;

            skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Lerp(100, 0, currentTime / timeToClose));
            yield return null;
        }
        yield return null;
    }


    IEnumerator Elevate()
    {
        playerRefrence.transform.parent = transform;
        if (!autoStart)
        {
            StartCoroutine("CloseDoorAnim");
            yield return new WaitForSeconds(timeToClose);
        }

        while(Vector3.Distance(transform.position, target.transform.position) > 0.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            yield return null;
        }
      
        StartCoroutine("OpenDoorAnim");
        playerRefrence.transform.parent = null;
        yield return null;
    }
    [ContextMenu("StartElevator")]
    public void StartElevator()
    {
        StartCoroutine("Elevate");
    }
    private void OnTriggerEnter(Collider other)
    {
        StartElevator();
    }

}
