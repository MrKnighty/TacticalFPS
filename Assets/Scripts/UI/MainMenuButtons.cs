using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
   public void StartButton(int index)
    {
     
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}
