using UnityEngine;

[ExecuteInEditMode]
public class ZoomShaderScreenPosition : MonoBehaviour
{
    // https://www.youtube.com/watch?v=IC5JoS0wX0s - Taken From CODEMONKEY
    [SerializeField] Material material;
    
    void Update()
    {
        Vector2 screenPixels = Camera.main.WorldToScreenPoint(transform.position);
        screenPixels = new Vector2(screenPixels.x / Screen.width, screenPixels.y / Screen.height);

        material.SetVector("_ObjectScreenPosition", screenPixels);        
    }
}
