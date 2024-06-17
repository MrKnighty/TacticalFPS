using UnityEngine;
using UnityEditor;
using Unity.AI;
using UnityEngine.AI;
using System;
using UnityEditor.TerrainTools;

public class AIBase : MonoBehaviour
{
    Transform playerTransform;
    NavMeshAgent agent;
    [Header("Shooting")]
    [SerializeField] GameObject bulletSparkFX;
    [SerializeField] protected float shootRadius = 2f;
    [SerializeField] protected float attackSpeed = 0.1f;
    [SerializeField] protected float ammoCap = 20f;
    [SerializeField] protected float reloadTime = 2f;
    [Header("Raycast Points")]
    [SerializeField] Transform aIHeadPoint;
    [SerializeField] Transform playerBodyPoint;
    [SerializeField] Transform playerHeadPoint;
    [Header("Cover")]
    [SerializeField] bool inCover = false;
    [SerializeField] float closestDistanceToCoverXZ;
    [SerializeField] float closestDistanceToCoverY;
    [SerializeField] float maximumDistanceToCoverXZ;
    [SerializeField] float maximumDistanceToCoverY;
    [SerializeField] LayerMask coverLayerMask;
    [Header("Patrol")]
    [SerializeField] Vector3[] patrolPoints;
    [SerializeField] bool onClickCreatePatrolPoint = true;
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if(patrolPoints.Length > 0)
        foreach (Vector3 p in patrolPoints) 
        {
            Gizmos.DrawWireSphere(p, 0.25f);
        }
    }
    [ContextMenu("EnableSpawnSphereOnHit")]
    private void EnableSpawnSphereOnHit()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    [ContextMenu("DisableSpawnSphereOnHit")]
    private void DisableSpawnSphereOnHit()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    void OnSceneGUI(SceneView sceneView)
    {
        if (onClickCreatePatrolPoint && Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
        {
            CreatePatrolPoint();
        }
    }


    void CreatePatrolPoint()
    {

        print("works");
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, float.MaxValue))
        {
            Array.Resize(ref patrolPoints, patrolPoints.Length + 1);
            patrolPoints[patrolPoints.Length - 1] = hit.point;
        }
        else
        {
            Debug.Log("Failed to create patrol point - no valid hit.");
        }
    }

#endif
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

    virtual protected Transform FindClosestCover()
    {
        for (int i = 0; i < 2; i++) //loop 1 = close area check - if no cover - loop 2 = max area check
        { 
            Collider[] hits = Physics.OverlapBox(transform.position, new Vector3(maximumDistanceToCoverXZ * i + closestDistanceToCoverXZ, maximumDistanceToCoverXZ * i + closestDistanceToCoverXZ, maximumDistanceToCoverY * i + closestDistanceToCoverY), Quaternion.identity, coverLayerMask);
            if(hits.Length > 1) 
            {
                Collider closestCollider = null;
                float closestDistance = Mathf.Infinity;
                float check;
                foreach (Collider c in hits) // Distance Check for all hits
                {
                    if (Vector3.Dot((hits[0].transform.position - playerTransform.position).normalized, hits[0].transform.position) < 0) //Check if the player is infront of the cover
                        continue;
                    check = Vector3.Distance(transform.position, c.transform.position);
                    if (check < closestDistance)
                    {
                        closestDistance = check;
                        closestCollider = c;
                    }
                }

                if (closestCollider != null)
                    return closestCollider.transform;
            }
            if(hits.Length == 1)
            {
                if (Vector3.Dot((hits[0].transform.position - playerTransform.position).normalized, hits[0].transform.position) > 0) //Check if the player is infront of the cover
                    return hits[0].transform;
            }
        }
        return null;
    }

    virtual protected  Vector3 GetCoverPointOnNavMesh(Transform coverTransform)
    {
        Cover cover = coverTransform.GetComponent<Cover>();
        Vector3 point = cover.GetPoint();
        Vector3 area = cover.size;
        Vector3 randomPoint = new Vector3(UnityEngine.Random.Range(point.x - area.x * 0.5f, point.x + area.x * 0.5f), point.y, UnityEngine.Random.Range(point.z - area.z * 0.5f, point.z + area.z * 0.5f));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
              
        //Vector3 pos = NavMesh.SamplePosition()
    }

    virtual protected Transform CanSeePlayer() // Returns an int based on if the AI can see the players head(2) body(1), cant see them (0), not in their 180deg view(3)
    {
        RaycastHit hit = new RaycastHit();
        Vector3 bodyDir = playerBodyPoint.position - aIHeadPoint.position;
        if (Vector3.Dot(bodyDir.normalized, transform.forward) < 0)
        {
            return null;
        }
        if (Physics.Raycast(aIHeadPoint.position, (bodyDir), out hit))
        {
           if(hit.transform.tag == "Player")
                return playerBodyPoint;
        }
        if(Physics.Raycast(aIHeadPoint.position, (playerHeadPoint.position - aIHeadPoint.position), out hit))
        {
            if (hit.transform.tag == "Player")
                return playerHeadPoint;
        }
        //Debug.DrawRay(aIHeadPoint.position, bodyDir);

        return null;
    }

    virtual protected bool ShootAtTarget(Vector3 point, float radius)
    {
        float halfRadius = radius / 2;
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-halfRadius, halfRadius), UnityEngine.Random.Range(-halfRadius, halfRadius), UnityEngine.Random.Range(-halfRadius, halfRadius));
        Vector3 alteredPoint = new Vector3(point.x + randomOffset.x, point.y + randomOffset.y, point.z + randomOffset.z);
        Vector3 direction = alteredPoint - aIHeadPoint.position;
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(aIHeadPoint.position, direction, out hit))
        {
            Instantiate(bulletSparkFX, hit.point, Quaternion.identity).transform.LookAt(transform);
            if (hit.transform.tag == "Player")
            {
                return true;
            }
        }
        return false;

    }
    void DebugStuff()
    {

    }
    float timer = 0;
    void Update()
    {
        print(CanSeePlayer());
        if (timer < attackSpeed)
        {
            timer += Time.deltaTime;
            return;
        }
        timer = 0;
        Transform point = CanSeePlayer();
        if (point)
            ShootAtTarget(point.position, shootRadius);
    }
}
