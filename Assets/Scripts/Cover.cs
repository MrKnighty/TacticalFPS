using UnityEngine;

public class Cover : MonoBehaviour
{
    public Vector3 areaOffset;
    public Vector3 size;
    public Vector3 GetPoint()
    {
        return areaOffset +transform.position;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(areaOffset + transform.position, size);
    }
}
