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
    Text pointText;

    [SerializeField]
    GameObject[] pointerArrow; // 0 = right ; 1 = left

    [SerializeField]
    GameObject goal;
    #endregion

    #region Fields
    private int points = 0;
    private int ConsecutiveGoals;
    private GameType gameType;
    private Ball ball;

    private const int THROW_TRIES_TOTAL = 6;
    private int throwTriesCurrent;

    float directionNumber;

    Renderer goalRenderer;
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
        pointText.text = "Points: " + points;
        pointerArrow[0].SetActive(false);
        pointerArrow[1].SetActive(false);

        throwTriesCurrent = THROW_TRIES_TOTAL;

        goalRenderer = goal.GetComponent<Renderer>();
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
                pointerArrow[0].SetActive(true);
                pointerArrow[1].SetActive(false);
            }
            else if (directionNumber == -1f) // If the goal is closer to the left.
            {
                pointerArrow[0].SetActive(false);
                pointerArrow[1].SetActive(true);
            }
        }
        else if (goalRenderer.isVisible) // If the goal is visible on the screen.
        {
            pointerArrow[0].SetActive(false);
            pointerArrow[1].SetActive(false);
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
        points += (10 + (ConsecutiveGoals * 10));
        pointText.text = "Points: " + points;
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
        SceneManager.LoadScene(0); // Returns to the map.
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
}
