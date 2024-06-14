using UnityEngine;
using Unity.AI;
using UnityEngine.AI;

public class AIBase : MonoBehaviour
{
    Transform[] patrolPoints;
    Transform playerTransform;
    NavMeshAgent agent;
    [Header("Raycast Points")]
    [SerializeField] Transform aIHeadPoint;
    [SerializeField] Transform playerBodyPoint;
    [SerializeField] Transform playerHeadPoint;
    [Header("Cover")]
    [SerializeField] bool inCover = false;
    [SerializeField] float maximumDistanceToCoverXZ;
    [SerializeField] float maximumDistanceToCoverY;
    [SerializeField] LayerMask coverLayerMask;
    virtual protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = FindAnyObjectByType<PlayerDamageHandler>().transform;
    }

    virtual protected Transform FindCover()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, new Vector3(maximumDistanceToCoverXZ, maximumDistanceToCoverXZ, maximumDistanceToCoverY), Quaternion.identity, coverLayerMask);
        return hits[0].transform;
    }

    virtual protected int CanSeePlayer() // Returns an int based on if the AI can see the players head(2) body(1), cant see them (0), not in their 180deg view(3)
    {
        RaycastHit hit = new RaycastHit();
        Vector3 bodyDir = playerBodyPoint.position - aIHeadPoint.position;
        if (Vector3.Dot(bodyDir.normalized, transform.forward) < 0)
        {
            return 3;
        }
        if (Physics.Raycast(aIHeadPoint.position, (bodyDir), out hit))
        {
           if(hit.transform.tag == "Player")
                return 1;
        }
        if(Physics.Raycast(aIHeadPoint.position, (playerHeadPoint.position - aIHeadPoint.position), out hit))
        {
            if (hit.transform.tag == "Player")
                return 2;
        }
        //Debug.DrawRay(aIHeadPoint.position, bodyDir);
            print(hit.transform.name);

        return 0;
            
    }
    
    void Update()
    {
       print( CanSeePlayer());
    }
}
