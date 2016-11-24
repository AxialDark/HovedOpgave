using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour {

    private static UIController instance;
    public Button btnStartGame;
    public Image pnlEndRoute;

    public static UIController Instance
    {
        get
        {
            if (instance == null) instance = GameObject.Find("UIController").GetComponent<UIController>();

            return instance;
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
