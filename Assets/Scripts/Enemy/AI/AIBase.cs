using UnityEngine;
using UnityEditor;
using Unity.AI;
using UnityEngine.AI;
using System;
using System.Collections;

public enum AIStates {Patrol, Gaurd, Aggro, Chase} 
public class AIBase : MonoBehaviour
{
    protected Transform playerTransform;
    protected PlayerDamageHandler playerDamageHandler;
    protected NavMeshAgent agent;
    [SerializeField] protected AIStates currentState;
    [Header("Shooting")]
    [SerializeField] GameObject bulletSparkFX;
    [SerializeField] protected float shootRadius = 2f;
    [SerializeField] protected float attackSpeed = 0.1f;
    [SerializeField] protected float reactionSpeed = 0.25f;
    [SerializeField] protected int ammoCap = 20;
    [SerializeField] protected float damage = 1;
    protected int ammo;
    [SerializeField] protected float reloadTime = 2f;
    [Header("Raycast Points")]
    [SerializeField] Transform aIHeadPoint;
    [SerializeField] Transform playerBodyPoint;
    [SerializeField] Transform playerHeadPoint;
    [Header("Cover")]
    [SerializeField] protected bool inCover = false;
    [SerializeField] float closestDistanceToCoverXZ;
    [SerializeField] float closestDistanceToCoverY;
    [SerializeField] float maximumDistanceToCoverXZ;
    [SerializeField] float maximumDistanceToCoverY;
    [SerializeField] protected float guardTime = 10;
    [SerializeField] LayerMask coverLayerMask;
    [Header("Patrol")]
    [SerializeField] protected Vector3[] patrolPoints;
    [SerializeField] protected Vector3[] patrolDirections;
    [SerializeField] bool onClickCreatePatrolPoint = true;
    [SerializeField] protected float patrolWaitTimer;
    [SerializeField] protected float patrolTurnSpeed;
    [SerializeField] protected bool isPaused = false;
    [Header("Audio")]
    [SerializeField] protected AudioClip shootSFX;
    protected AudioSource audioSource;
    virtual protected void Start()
    {
        audioSource = this?.GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        playerDamageHandler = FindAnyObjectByType<PlayerDamageHandler>();
        playerTransform = playerDamageHandler.transform;
        ammo = ammoCap;
        if (!playerBodyPoint)
        { 
            foreach(Transform t in playerTransform.GetComponentsInChildren<Transform>())
            {
                if (t.name == "BodyPoint")
                    playerBodyPoint = t;
                else if(t.name == "HeadPoint")
                    playerHeadPoint = t;
            }
        }

    }
    virtual public void DamageTrigger()
    {
        
    }

    virtual protected Transform FindCover()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, new Vector3(maximumDistanceToCoverXZ, maximumDistanceToCoverXZ, maximumDistanceToCoverY), Quaternion.identity, coverLayerMask);
        return hits[0].transform;
    }

    virtual protected Transform FindClosestCover(Transform currentCover = null, bool canSeePlayer = true)
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
                    if (canSeePlayer && Vector3.Dot(playerTransform.position, hits[0].transform.position) < 0) //Check if the player is infront of the cover
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
        if(coverTransform == null) return Vector3.zero;
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
            audioSource.PlayOneShot(shootSFX, 1 * GameControllsManager.audioVolume);
            if (hit.transform.tag == "Player")
            {
                return true;
            }
            Instantiate(bulletSparkFX, hit.point, Quaternion.identity).transform.LookAt(transform);
        }
        return false;

    }
    protected IEnumerator Patrol()
    {
        if (patrolPoints.Length == 0)
            yield break;
        int i = 0;
        foreach (Vector3 p in patrolPoints)
        {
            agent.isStopped = false;
            agent.destination = p;
            while (Vector3.Distance(transform.position, agent.destination) > 0.02f && currentState == AIStates.Patrol)
            {
                yield return new WaitForSeconds(0.1f);
            }
            if (AIStates.Patrol != currentState)
                break;
            agent.isStopped = true;
            if (patrolDirections[i] != Vector3.zero)
            {
                float waitTimer = 0;
                //Quaternion originalRotation = transform.rotation;
                Quaternion dir = Quaternion.LookRotation(patrolDirections[i]);
                while (waitTimer < patrolWaitTimer)
                {
                    if (AIStates.Patrol != currentState)
                        break;
                    if (isPaused)
                        yield return null;
                    waitTimer += Time.deltaTime;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, dir, patrolTurnSpeed * Time.deltaTime);
                    yield return null;
                }
            }
            i++;
        }
        if (AIStates.Patrol == currentState)
            StartCoroutine(Patrol());
            
    }

    protected IEnumerator Timer(float duration)
    {
        if(duration == 0)
        {
            yield return null;
        }
        else
        {
            float timer = 0;
            while (timer < duration)
            {
                if (isPaused) yield return null;
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (patrolPoints.Length > 0)
        {
            foreach (Vector3 p in patrolPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(p, 0.25f);
            }
            for (int i = 0; i < patrolDirections.Length; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(patrolPoints[i], patrolDirections[i]);
            }
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
    bool OnKeyUpLatch = true;
    void OnSceneGUI(SceneView sceneView)
    {
        if (OnKeyUpLatch)
            if (onClickCreatePatrolPoint && Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
            {
                OnKeyUpLatch = false;
                CreatePatrolPoint();
            }
        if (onClickCreatePatrolPoint && Event.current != null && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Space)
        {
            OnKeyUpLatch = true;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, float.MaxValue))
            {
                if (Vector3.Distance(patrolPoints[patrolPoints.Length - 1], hit.point) > 0.5f)
                {
                    Array.Resize(ref patrolDirections, patrolDirections.Length + 1);
                    patrolDirections[patrolDirections.Length - 1] = hit.point - patrolPoints[patrolDirections.Length - 1];
                }
                else
                {
                    Array.Resize(ref patrolDirections, patrolDirections.Length + 1);
                    patrolDirections[patrolDirections.Length - 1] = Vector3.zero;
                }
            }

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
}
