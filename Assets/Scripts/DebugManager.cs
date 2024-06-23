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
    

    private void Start()
    {
        debugUIGO = spawnableDebugUI;
        canvas = canvasObject;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            DebugMode = !DebugMode;
       
            
    }
    public static void DisplayInfo(string identifyer, string message)
    {
        if (!DebugMode)
            return;
        if(debugUI.ContainsKey(identifyer))
        {
            debugUI[identifyer].text = message;
        }
        else
        {
            GameObject ui =  Instantiate(debugUIGO);
            TextMeshProUGUI tmpro = ui.GetComponent<TextMeshProUGUI>();
            ui.transform.SetParent(  canvas.transform,false);
            debugUI.Add(identifyer, tmpro);
            tmpro.rectTransform.localPosition = new Vector3(10, positionOffset, 0);
            positionOffset -= 40;
        }
    }
}
