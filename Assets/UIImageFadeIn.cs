using UnityEngine;
using UnityEngine.UI;


public class UIImageFadeIn : MonoBehaviour
{
    Image image;
    Color color1;
    Color color2;
    [SerializeField] float fadeInTime = 1;
    void Start()
    {
        image = GetComponent<Image>();
        image.enabled = true;
        color1 = image.color;
        color2 = new Color(color1.r, color1.g, color1.b, 0);
    }

    float timer = 0;
    void Update()
    {
        timer += Time.deltaTime / fadeInTime;
        image.color = Color.Lerp(color1, color2, timer);
        if(timer >= 1)
        {
            enabled = false;
        }
    }
}
