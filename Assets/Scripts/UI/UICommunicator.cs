using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UICommunicator : MonoBehaviour
{

    static Dictionary<string, TextMeshProUGUI> ui;
    [SerializeField] TextMeshProUGUI[] uiElemets;

    [SerializeField] GameObject pauseMenu, gameOverMenu, debugControlls;
    


    public static UICommunicator refrence;
    public static bool gamePaused = false;

    [SerializeField] GameObject popupTextObject;
    [SerializeField] Vector2 startMessageSpot, endMessageSpot;
   
    
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

        if(Input.GetKeyDown(KeyCode.F11))
        {
            PopupText("Blah Blah Blah1",  0.2f);
        }
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
            debugControlls.SetActive(false);
            PlayerPrefs.Save();

        }
        else
        {
            SwitchToUI(true);
            pauseMenu.SetActive(true);
            if(DebugManager.DebugMode)
            debugControlls.SetActive(true);
        
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
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


    public void RestartLevel()
    {
        SwitchToUI(false);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public void PopupText(string message, float messageTime)
    {
        GameObject ui = Instantiate(popupTextObject);
        TextMeshProUGUI text = ui.GetComponent<TextMeshProUGUI>();
        text.text = message;
        text.rectTransform.position = startMessageSpot;
        ui.transform.SetParent(this.transform, true);
        StartCoroutine(MessageFloatUp( messageTime, text));
    }

    IEnumerator MessageFloatUp(float messageTime, TextMeshProUGUI element)
    {
        float currentTime = 0;
        float alphaLerp = 0;
        Color textColor = element.color;
        while(currentTime < messageTime)
        {
            currentTime += Time.deltaTime;
            element.rectTransform.position = Vector3.Lerp(startMessageSpot, endMessageSpot, currentTime / messageTime);
    
           
                textColor.a = Mathf.Lerp(1,0, currentTime / messageTime);
                alphaLerp += Time.deltaTime; 
                element.color = textColor;
            
            yield return null;
        }
        Destroy(element.gameObject);
        yield break;
    }
   





}
