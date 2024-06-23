#if UNITY_EDITOR
using UnityEngine;

public class GizmosDrawer : MonoBehaviour
{
    [SerializeField] float radius = 0.01f;
    [SerializeField] Color color = Color.black;
    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
    }

  
    
}

#endif