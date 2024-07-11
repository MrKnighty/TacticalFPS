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
        Gizmos.matrix = transform.localToWorldMatrix; //Liam you need to set the matrix in order to get rotation :)
        Gizmos.DrawCube(Vector3.zero, GetComponent<BoxCollider>().size); //Got rid of the the scaling since the local matrix is default now
    }
}
