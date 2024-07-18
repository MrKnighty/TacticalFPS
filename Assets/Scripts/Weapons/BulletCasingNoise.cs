using UnityEngine;

public class BulletCasingNoise : MonoBehaviour
{
    [SerializeField] AudioClip[] inpactSounds;
    [SerializeField] AudioSource source;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag != "BulletCasing")
       source.PlayOneShot(inpactSounds[Random.Range(0, inpactSounds.Length - 1)], GameControllsManager.audioVolume * 0.7f);
    }
}
