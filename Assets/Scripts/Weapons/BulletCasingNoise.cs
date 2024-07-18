using UnityEngine;

public class BulletCasingNoise : MonoBehaviour
{
    [SerializeField] AudioClip[] inpactSounds;
    [SerializeField] AudioSource source;
    bool canMakeSound = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!canMakeSound || collision.gameObject.layer == 9)//9 is the layer of the casing
            return;
    
        canMakeSound = false;
        source.PlayOneShot(inpactSounds[Random.Range(0, inpactSounds.Length - 1)], GameControllsManager.audioVolume * 0.7f);
        Invoke("TimerOver", 0.25f);
    }


    void TimerOver()
    {
        canMakeSound = true;
    }
}
