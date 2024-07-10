using UnityEngine;

public class ActivateEnemiesEvent : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;

    private void Start()
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        foreach(GameObject enemy in enemies)
        {
            enemy.SetActive(true);
        }
            Destroy(this);
    }
}
