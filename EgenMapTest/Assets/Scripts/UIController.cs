using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

/// <summary>
/// In-scene singleton for UI elements not part of a game activity and scene transistion
/// </summary>
public class UIController : MonoBehaviour
{

    private static UIController instance;
    public Button btnStartGame;
    public Image pnlEndRoute;
    public Button btnDebugStartRoute;
    public Button btnDebugEndRoute;

    /// <summary>
    /// Unity Singleton
    /// </summary>
    public static UIController Instance
    {
        get
        {
            if (instance == null) instance = GameObject.Find("UIController").GetComponent<UIController>();

            return instance;
        }
    }

    /// <summary>
    /// Unity method called every time game object is enabled
    /// </summary>
    void OnEnable()
    {
#if UNITY_ANDROID
        btnDebugStartRoute.gameObject.SetActive(true);
        btnDebugEndRoute.gameObject.SetActive(true);
#endif
    }

    /// <summary>
    /// Add new scene to the scene hierarchy and then activate that scene
    /// </summary>
    /// <param name="sceneName">Name of scene</param>
    public void LoadScene(string _sceneName)
    {
        StartCoroutine(LoadNextScene(_sceneName));
    }

    /// <summary>
    /// Coroutine to load a scene
    /// </summary>
    /// <param name="_sceneName">Name of of scene</param>
    /// <returns></returns>
    private IEnumerator LoadNextScene(string _sceneName)
    {
        print("Load Next Scene Started");
        AsyncOperation _async = new AsyncOperation();
        _async = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        _async.allowSceneActivation = false;

        while (!_async.isDone)
        {
            yield return null;
            if (_async.progress >= 0.9f)
            {
                _async.allowSceneActivation = true;
            }
        }

        Scene nextScene = SceneManager.GetSceneByName(_sceneName);
        if (nextScene.IsValid())
        {
            if (nextScene.name == "GameScene")
            {
                GameObject.FindGameObjectWithTag("AllMainObjects").SetActive(false);
                btnStartGame.gameObject.SetActive(false);
            }
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(nextScene);
            print("Loaded next scene");
        }
    }

    /// <summary>
    /// Button Click Method.
    /// Generates a route
    /// </summary>
    public void ClickStartRoute()
    {
        DebugRouting.RandomRouting(RouteLength.MIDDLE);
        //DebugRouting.RandomRoute();
    }

    /// <summary>
    /// Button Click Method
    /// Ends a route
    /// </summary>
    public void ClickEndRoute()
    {
        RouteManager.EndRoute();
    }
}
