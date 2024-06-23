using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UICommunicator : MonoBehaviour
{

    static Dictionary<string, TextMeshProUGUI> ui;
    [SerializeField] TextMeshProUGUI[] uiElemets;

    [SerializeField] GameObject pauseMenu, gameOverMenu;
    [SerializeField] Slider audioSlider;

    static UICommunicator refrence;
    public static bool gamePaused = false;
    public static float audioLevel = 1f;
    void Start()
    {
        ui = new Dictionary<string, TextMeshProUGUI>();
        foreach (TextMeshProUGUI gui in uiElemets)
        {
            ui.Add(gui.name, gui);

        }
        refrence = this;
        SwitchToUI(false);
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

    void SwitchToUI(bool toggle)
    {
        if (!toggle)
        {
            Time.timeScale = 1.0f;
            gamePaused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Time.timeScale = 0.0f;
            gamePaused = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void PauseUI() // this will do all of the pausing for now, but ill move it to a proper game manager later
    {
        if (gamePaused)
        {
            SwitchToUI(false);
            pauseMenu.SetActive(false);
       
        }
        else
        {
            SwitchToUI(true);
            pauseMenu.SetActive(true);
        
        }
    }
    public void GameOverUI()
    {
        SwitchToUI(true);
        gameOverMenu.SetActive(true);
    }

    public void MainMenu()
    {
        SwitchToUI(true);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void UpdateAudio()
    {
        audioLevel = audioSlider.value;
     
    }
    public void RestartLevel()
    {
        SwitchToUI(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
   





}
