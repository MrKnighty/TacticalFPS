using UnityEngine;

public class Cover : MonoBehaviour
{
    public Vector3 areaOffset;
    public Vector3 size;
    [SerializeField] bool setLayerMask = true;
    private void Start()
    {
        
        if (gameObject.layer != LayerMask.NameToLayer("Cover"))
        {
            gameObject.layer = LayerMask.NameToLayer("Cover");
        }

    }
    public Vector3 GetPoint()
    {
        return areaOffset +transform.position;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(areaOffset + transform.position, size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(areaOffset + transform.position, 0.1f);
    }
}
