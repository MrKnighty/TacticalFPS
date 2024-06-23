using UnityEngine;

public class ExitGame : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
