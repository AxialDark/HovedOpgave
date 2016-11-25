using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// In-scene singleton for UI elements not part of a game activity and scene transistion
/// </summary>
public class UIController : MonoBehaviour {

    private static UIController instance;
    public Button btnStartGame;
    public Image pnlEndRoute;

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
    /// Loads new scene
    /// </summary>
    /// <param name="sceneName">Name of scene</param>
    public void LoadScene(string _sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
