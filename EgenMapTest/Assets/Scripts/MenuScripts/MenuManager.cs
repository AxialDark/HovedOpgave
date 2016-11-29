using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {


    /*
     * 0 = Startscreen
     * 1 = Create User Menu
     * 2 = Head Menu
     * 3 = Statistic Menu
     * 4 = Challengemenu
     * 5 = Find Route Menu
     * 6 = Map
     * 7 = End Game Screen
     */
    [SerializeField]
    GameObject[] Panels;

	// Use this for initialization
	void Start () 
    {
        SetActiveMenu(0);
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    public void OnClickLogin()
    {
        SetActiveMenu(2); //To Headmenu
    }

    public void OnClickCreateUser()
    {
        SetActiveMenu(1); //To Create User Menu
    }

    public void OnClickCreateUserComplete()
    {
        SetActiveMenu(0); //To Startscreen
    }

    public void OnClickStatistics()
    {
        SetActiveMenu(3); //To Statistic Menu
    }

    public void OnClickChallenges()
    {
        SetActiveMenu(4); //To Challengemenu
    }

    public void OnClickChallengeAccept()
    {
        SetActiveMenu(6); //To Map
    }

    public void OnClickFindRoute()
    {
        SetActiveMenu(5); //To Find Route Menu
    }

    public void OnCLickMakeRoute()
    {
        SetActiveMenu(6); //To Map
    }

    public void OnclickDeclineRoute()
    {
        SetActiveMenu(5); //To Find Route Menu
    }

    public void OnclickDone()
    {
        SetActiveMenu(0); //To Find Route Menu
    }

    public void OnClickStartGame()
    {
        SetActiveMenu(8);
        SceneManager.LoadScene(1);
    }

    private void SetActiveMenu(int menuIndex)
    {
        Panels[0].SetActive(false);
        Panels[1].SetActive(false);
        Panels[2].SetActive(false);
        Panels[3].SetActive(false);
        Panels[4].SetActive(false);
        Panels[5].SetActive(false);
        Panels[6].SetActive(false);
        Panels[7].SetActive(false);

        if (menuIndex < 8)
        {
            Panels[menuIndex].SetActive(true);
        }
    }
}
