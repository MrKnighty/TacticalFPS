using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
public class DebugManager : MonoBehaviour 
{
    public static bool DebugMode = false;

    static Dictionary<string, TextMeshProUGUI> debugUI = new Dictionary<string, TextMeshProUGUI>();
    [SerializeField] GameObject spawnableDebugUI;
    [SerializeField] GameObject canvasObject;

    static GameObject debugUIGO;
  
    static GameObject canvas;
    static int positionOffset = -10;

    float fpsUpdateTime = 0.05f;
    float timer;
    

    private void Start()
    {
        debugUI.Clear();
        debugUIGO = spawnableDebugUI;
        canvas = canvasObject;
        positionOffset = 0;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            DebugMode = !DebugMode;

        if(Input.GetKeyDown(KeyCode.F4))
        {
            Application.targetFrameRate = 999;
            UICommunicator.refrence.PopupText("FPS UNCAPPED", 2);
        }
        if(Input.GetKeyDown(KeyCode.F5))
        {
            Application.targetFrameRate = 20;
            UICommunicator.refrence.PopupText("FPS Capped!", 2);
        }
        timer -= Time.deltaTime;
        if(timer <=0)
        {
            timer = fpsUpdateTime;
            DisplayInfo("FPS",((int)(1.0f / Time.deltaTime)).ToString(), true);
        }
       
            
    }

   static bool release = true;
    public static void DisplayInfo(string identifyer, string message, bool ignoreDebugMode)
    {
        if (release && !DebugMode)
            return;

        if (debugUI.ContainsKey(identifyer))
        {
            debugUI[identifyer].text = message;
        }
        else
        {
            GameObject ui = Instantiate(debugUIGO);
            TextMeshProUGUI tmpro = ui.GetComponent<TextMeshProUGUI>();
            ui.transform.SetParent(canvas.transform, false);
            debugUI.Add(identifyer, tmpro);
            tmpro.rectTransform.localPosition = new Vector3(10, positionOffset, 0);
            positionOffset -= 40;
        }
    }
    public static void DisplayInfo(string identifyer, string message)
    {
        if (!DebugMode)
            return;

        DisplayInfo(identifyer, message, true);
    }
}
