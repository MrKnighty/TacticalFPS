using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadLevelTrigger : MonoBehaviour
{
    [SerializeField] bool initiateLoad;
    [SerializeField] bool load;

    [SerializeField] int levelindex;

    static bool allowedToLoad;
    static bool allreadyLoading;
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.transform.tag == "Player")
        {
            if (initiateLoad && !allreadyLoading)
                StartCoroutine("LoadScene");
            if(load)
                allowedToLoad = true;

        }
    }


   

    IEnumerator LoadScene()
    {
        AsyncOperation sceneLoader = SceneManager.LoadSceneAsync(levelindex);
        sceneLoader.allowSceneActivation = false;
        allreadyLoading = true;
        while (!sceneLoader.isDone)
        {
            DebugManager.DisplayInfo("Asyunc", "Scene Load %" + sceneLoader.progress * 100);

            if(sceneLoader.progress > 0.9f)
                DebugManager.DisplayInfo("Asyunc", "Scene Primed!" + sceneLoader.progress);

            if (allowedToLoad && sceneLoader.progress >= 0.9f)
            {
                FindAnyObjectByType<PresistantPlayerData>().Save();
                sceneLoader.allowSceneActivation = true;

                
            }

            yield return null;
        }



        yield return null;
    }



}
