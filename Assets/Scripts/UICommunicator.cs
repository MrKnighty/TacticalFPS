using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class UICommunicator : MonoBehaviour
{

    static Dictionary<string, TextMeshProUGUI> ui;
    [SerializeField] TextMeshProUGUI[] uiElemets;

    [SerializeField] GameObject pauseMenu, gameOverMenu;

    static UICommunicator refrence;
    public static bool gamePaused = false;
    void Start()
    {
        ui = new Dictionary<string, TextMeshProUGUI>();
        foreach (TextMeshProUGUI gui in uiElemets)
        {
            ui.Add(gui.name, gui);

        }
        refrence = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseUI();
    }

    public static void UpdateUI(string id, string Message)
    {
        
        if (!ui.ContainsKey(id))
        {
            print("Warning! this ui does not exist!");
            return;
        }
        ui[id].text = Message;
    }

    public void PauseUI() // this will do all of the pausing for now, but ill move it to a proper game manager later
    {
        if (gamePaused)
        {
            Time.timeScale = 1.0f;
            pauseMenu.SetActive(false);
            gamePaused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Time.timeScale = 0.0f;
            pauseMenu.SetActive(true);
            gamePaused = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
   





}
