using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    #endregion

    #region Serialized Fields
    [SerializeField]
    private Text pointText;
    [SerializeField]
    private GameObject[] pointerArrows; // 0 = right ; 1 = left
    [SerializeField]
    private GameObject goal;
    [SerializeField]
    private Image throwGoalPanel;
    [SerializeField]
    private Sprite[] throwMissSprites; // 0 = standard ; 1 = goal ; 2 = miss
    #endregion

    #region Fields
    private int point = 0;
    private int consecutiveGoalCount;
    private GameType gameType;
    private Ball ball;
    private const int THROW_TRIES_TOTAL = 6;
    private int throwTriesCurrent;
    private float directionNumber;
    private Renderer goalRenderer;
    private Image[] throwMissIcons;
    private int throwIndex;
    #endregion

    /// <summary>
    /// Unity Mehtod, called at the very beginning, before Start().
    /// </summary>
	private void Awake()
    {
        Initialize();
    }
    
    /// <summary>
    /// Method used for initialization.
    /// </summary>
    private void Initialize()
    {
        pointText.text = "Points: " + point;
        pointerArrows[0].SetActive(false);
        pointerArrows[1].SetActive(false);

        throwTriesCurrent = THROW_TRIES_TOTAL;

        goalRenderer = goal.GetComponent<MeshRenderer>();

        throwMissIcons = throwGoalPanel.GetComponentsInChildren<Image>();
        throwIndex = 1;

        UIController.Instance.ShowLoading(false);
    }

    /// <summary>
    /// Unity method, Update is called once per frame.
    /// </summary>
    private void Update()
    {
        Vector3 targetDirection = goal.transform.position - Camera.main.transform.position; // Vector from the camera to the goal.
        directionNumber = AngleDir(Camera.main.transform.forward, targetDirection, Camera.main.transform.up);

        if (!goalRenderer.isVisible) // If the goal is not visible on the screen.
        {
            if (directionNumber == 1f) // If the goal is closer to the right.
            {
                pointerArrows[0].SetActive(true);
                pointerArrows[1].SetActive(false);
            }
            else if (directionNumber == -1f) // If the goal is closer to the left.
            {
                pointerArrows[0].SetActive(false);
                pointerArrows[1].SetActive(true);
            }
        }
        else if (goalRenderer.isVisible) // If the goal is visible on the screen.
        {
            pointerArrows[0].SetActive(false);
            pointerArrows[1].SetActive(false);
        }

        if(throwTriesCurrent <= 0)
        {
            EndGame();
        }
    }

    /// <summary>
    /// Method that calculates and adds point for a goal.
    /// </summary>
    public void CalculateGoalPoints()
    {
        point += (10 + (consecutiveGoalCount * 10));
        pointText.text = "Points: " + point;
    }

    /// <summary>
    /// Increments the consecutiveGoalCount variable.
    /// </summary>
    public void AddConsecutiveGoal()
    {
        consecutiveGoalCount++;

        if(throwIndex < 6)
        ChangeThrowMissIcon(true);
    }

    /// <summary>
    /// Resets the consecutiveGoalCount variable back to 0.
    /// </summary>
    public void ResetConsecutiveGoals()
    {
        consecutiveGoalCount = 0;

        if(throwIndex < 6)
        ChangeThrowMissIcon(false);
    }

    /// <summary>
    /// Method for decreasing the amount of throws left by one.
    /// </summary>
    public void ThrowDeduct()
    {
        throwTriesCurrent--;
    }

    /// <summary>
    /// Method for handling the end of a game.
    /// </summary>
    private void EndGame()
    {
        PointManager.Instance.AddPoints(point);
        point = 0;
        consecutiveGoalCount = 0;
        throwIndex = 1;

        UIController.Instance.ActivateProfileButton();

        UIController.Instance.UnloadActiveScene();
        UIController.Instance.RefMainScene.SetActive(true);
    }

    /// <summary>
    /// Method for finding out if an object(A) is to the left or to the right of another objects(B) Vector3.Forward.
    /// </summary>
    /// <param name="_forward">The Vector3.forward of B.</param>
    /// <param name="_targetDirection">The Direction of the target A.</param>
    /// <param name="_up">The Vector3.Up of B</param>
    /// <returns>Float indicating wich direction the Object A is in relation to Object B: 1f = right; -1f = left; 0f = in front.</returns>
    private float AngleDir(Vector3 _forward, Vector3 _targetDirection, Vector3 _up)
    {
        Vector3 perp = Vector3.Cross(_forward, _targetDirection);
        float direction = Vector3.Dot(perp, _up);

        if (direction > 0f)
        {
            return 1f; // To the right
        }
        else if (direction < 0f)
        {
            return -1f; // To the left
        }
        else
        {
            return 0f; // In front
        }
    }

    /// <summary>
    /// Changes the icons for the throws, based on wether or not a goal was scored.
    /// </summary>
    /// <param name="_isGoal">Was a goal scored?</param>
    private void ChangeThrowMissIcon(bool _isGoal)
    {
        if (_isGoal) // If goal.
        {
            throwMissIcons[throwIndex].sprite = throwMissSprites[1];
        }
        else if (!_isGoal) // If miss.
        {
            throwMissIcons[throwIndex].sprite = throwMissSprites[2];
        }

        throwIndex++;
    }
}
