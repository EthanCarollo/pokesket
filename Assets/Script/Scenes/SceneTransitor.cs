using System;
using UnityEngine;

public class SceneTransitor : MonoBehaviour
{
    public static SceneTransitor Instance;
    
    public GameObject loadingScreen;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    
    public void LoadScene(int sceneToLoad){
        var loadingScreenPrefab = GameObject.Instantiate(loadingScreen);
        loadingScreenPrefab.GetComponent<LoadingScreenManager>().StartToLoadScene(sceneToLoad);
    }
    
    public void LoadScene(int sceneToLoad, Action<GameManager> onEndCallback){
        var loadingScreenPrefab = GameObject.Instantiate(loadingScreen);
        loadingScreenPrefab.GetComponent<LoadingScreenManager>().StartToLoadScene(sceneToLoad, onEndCallback);
    }
}
