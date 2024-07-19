using UnityEngine;

using UnityEngine.SceneManagement;
using System.Collections;

public class AsyncSceneLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugManager.DebugMode && Input.GetKeyDown(KeyCode.F6))
            StartCoroutine("LoadScene");
    }
    IEnumerator LoadScene()
    {
       AsyncOperation sceneLoader = SceneManager.LoadSceneAsync("MainLevel");
        sceneLoader.allowSceneActivation = false;

        while(!sceneLoader.isDone)
        {
            DebugManager.DisplayInfo("Asyunc", "Scene Load %" + sceneLoader.progress * 100);

            if (Input.GetKeyDown(KeyCode.F6) && sceneLoader.progress >= 0.9f )
            {
                FindAnyObjectByType<PresistantPlayerData>().Save();
                sceneLoader.allowSceneActivation = true;

                DebugManager.DisplayInfo("Asyunc", "Scene Primed!" + sceneLoader.progress);
            }
                
            yield return null;
        }

        

        yield return null;
    }
    
}
