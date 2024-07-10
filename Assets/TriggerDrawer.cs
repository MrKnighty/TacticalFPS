using UnityEngine;

public class TriggerDrawer : MonoBehaviour
{
    [SerializeField] Color color = Color.blue;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        Gizmos.DrawCube(transform.position,Vector3.Scale( GetComponent<BoxCollider>().size,  transform.localScale));
    }
}
