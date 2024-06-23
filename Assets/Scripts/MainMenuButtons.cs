using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
   public void StartButton()
    {
     
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}
