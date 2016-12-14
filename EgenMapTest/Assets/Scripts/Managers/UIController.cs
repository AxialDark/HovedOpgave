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
    [SerializeField]
    private GameObject pnlLoading;
    public bool show = false; //ONLY FOR DEBUG
    [SerializeField]
    private ErrorPanel pnlError;

    /// <summary>
    /// Shows the loading paneæ
    /// </summary>
    /// <param name="_show">Show or hide</param>
    public void ShowLoading(bool _show)
    {
        show = _show; //ONLY FOR DEBUG
        pnlLoading.SetActive(_show);
    }

    private WorldMap map = null;

    private static UIController instance;
    public Button btnStartGame;
    public Image pnlEndRoute;
    public Image pnlInGameMenu;
    public Image pnlChallengeMenu;
    public Image pnlRouteButtons;
    private GameObject refMainScene;
    [SerializeField]
    private Text timerText;
    [SerializeField]
    private Text EndStatsText;
    [SerializeField]
    private Button profileButton;

    public Button btnDebugStartRoute;
    public Button btnDebugEndRoute;
    public GameObject DebugPhoneMovement;


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
    /// A reference til the game object with all object in the main scene as children.
    /// Needed to reactivate the game object after a game has been played
    /// </summary>
    public GameObject RefMainScene
    {
        get
        {
            return refMainScene;
        }
    }
    /// <summary>
    /// Reference to the world map object in the main scene.
    /// Used to make UI affect the main scene
    /// </summary>
    public WorldMap Map
    {
        get
        {
            return map;
        }

        set
        {
            map = value;
        }
    }
    /// The GameLocation that the player hit
    /// </summary>
    public GameLocation HitLocation { get; private set; }


    private void Start()
    {
        pnlError.Initialize();
    }


    /// <summary>
    /// Unity method called every time game object is enabled
    /// </summary>
    void OnEnable()
    {
#if UNITY_ANDROID
        btnDebugEndRoute.gameObject.SetActive(true);
        //DebugPhoneMovement.gameObject.SetActive(true);
#endif
    }

    /// <summary>
    /// Unity method, runs once every frame
    /// </summary>
    private void Update()
    {
        timerText.text = PointManager.Instance.TimeToTimer();
    }

    /// <summary>
    /// Add new scene to the scene hierarchy and then activate that scene
    /// </summary>
    /// <param name="sceneName">Name of scene</param>
    public void LoadScene(string _sceneName)
    {
        if (_sceneName == "GameScene")
            ShowLoading(true);
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

        while (!_async.isDone) //Keep in leep as long as scene isn't loaded
        {
            yield return null;
            if (_async.progress >= 0.9f) //When a scene is 90% loaded, it needs allowSceneActivation as true to load the remaining 10%
            {
                _async.allowSceneActivation = true;
            }
        }

        Scene nextScene = SceneManager.GetSceneByName(_sceneName);
        if (nextScene.IsValid()) //Makes sure it's a valid scene
        {
            if (nextScene.name == "GameScene") //Is it the game scene that is being loaded?
            {
                refMainScene = GameObject.FindGameObjectWithTag("AllMainObjects"); //Find the parent to all objects in main scene. Needed when unloading from game scene
                refMainScene.SetActive(false); //Disable all objects in Main scene
                btnStartGame.gameObject.SetActive(false); //Hide button             
            }
            //Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(nextScene); //Activates the loaded scene
            print("Loaded next scene");
        }
    }

    /// <summary>
    /// Button Click Method.
    /// Generates a route
    /// </summary>
    public void ClickStartRoute(int _length)
    {
        if (Map == null) return;

        RouteLength length = 0;
        switch (_length)
        {
            case 2:
                length = RouteLength.SHORT;
                break;
            case 5:
                length = RouteLength.MIDDLE;
                break;
            case 10:
                length = RouteLength.LONG;
                break;
        }

        if (length == 0) return;
        Map.BeginRouteGeneration(length);
    }

    /// <summary>
    /// Button Click Method.
    /// Ends a route
    /// </summary>
    public void ClickEndRoute()
    {
        RouteManager.Instance.EndRoute();
    }

    /// <summary>
    /// Button Click Method.
    /// Confirms a selected challenge and start the process of generating the attached route
    /// </summary>
    /// <param name="obj">A scroll view's Content</param>
    public void ClickConfirmChallenge(GameObject obj)
    {
        Toggle[] toggles = obj.GetComponentsInChildren<Toggle>();

        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                Challenge challenge = toggles[i].GetComponentInChildren<Challenge>();

                Map.BeginRouteGeneration(challenge.routePoints, challenge.routelength);

                pnlChallengeMenu.gameObject.SetActive(false);
                pnlRouteButtons.gameObject.SetActive(false);
                break;
            }
        }
    }      

    /// <summary>
    /// Set the GameLocation as the HitLocation if null and shows the Start Game button
    /// </summary>
    /// <param name="_loc">The GameLocation hit</param>
    public void HitGameLocation(GameLocation _loc)
    {
        if (_loc == HitLocation)
            btnStartGame.gameObject.SetActive(true);

        if (HitLocation == null && _loc.RoutePoint == RouteManager.Instance.Points[1])
        {
            HitLocation = _loc;
            print("Hit location");
        }
    }

    /// <summary>
    /// Runs when Start Game button is clicked
    /// </summary>
    public void OnStartGameClick()
    {
        HitLocation.gameObject.SetActive(false);
        HitLocation = null;
    }

    /// <summary>
    /// Changes the end route statistic text
    /// </summary>
    /// <param name="_endStatText">The text to change the endStatText with</param>
    public void ChangeEndStatsText(string _endStatText)
    {
        EndStatsText.text = _endStatText;
    }

    /// <summary>
    /// Activates the profile button
    /// </summary>
    public void ActivateProfileButton()
    {
        profileButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Theme dropdown event method when choosing another color theme
    /// </summary>
    /// <param name="_val"></param>
    public void OnColorThemeChange(int _val)
    {
        if (map != null)
        {
            map.ChangeColorTheme((MapColorPalet)_val);
        }
    }
}
