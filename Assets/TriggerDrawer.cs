using UnityEngine;

public class TriggerDrawer : MonoBehaviour
{
    [SerializeField] Color color = new Color(0,0,1,0.128f);
    [SerializeField] bool render = true;
    private void OnDrawGizmos()
    {
        if (!render)
            return;

        Gizmos.color = color;

        Gizmos.DrawCube(transform.position,Vector3.Scale( GetComponent<BoxCollider>().size,  transform.localScale));
    }
}
