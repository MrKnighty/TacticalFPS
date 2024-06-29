using UnityEngine;

public class LightCuller : MonoBehaviour
{
    GameObject player;
    [SerializeField] float distance;
    Light myLight;

    private void Start()
    {
        
      
            
        myLight =  GetComponentInChildren<Light>();
    }

   
    
    void FixedUpdate()
    {
      
          if (Vector3.Distance(transform.position, PlayerController.playerInstance.transform.position) > distance)
            myLight.enabled = false;
        else
            myLight.enabled = true;
    }
}
