using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

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

    [SerializeField]
    Text pointText;

    #region Fields
    private int points = 0;
    private int ConsecutiveGoals;
    private GameType gameType;
    private Ball ball;
    #endregion

	private void Awake () 
    {
        Initialize();
	}

    private void Initialize()
    {
        pointText.text = "Points: " + points;
    }

    public void CalculateGoalPoints()
    {
        points += 10;
        pointText.text = "Points: " + points;
    }

    private void EndGame()
    {

    }
}
