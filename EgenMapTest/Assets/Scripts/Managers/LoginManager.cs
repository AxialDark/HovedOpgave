using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private InputField[] loginCreateInputFields; // 0 = Login username; 1 = Login password; 2 = Create username; 3 = Create password.

    [SerializeField]
    private Image startMenuPanel;

    [SerializeField]
    private Image createUserPanel;
    #endregion

    private Toaster toaster;

    /// <summary>
    /// Unity method, runs in the beginning before Start()
    /// </summary>
	private void Awake () 
    {
        toaster = new Toaster();
	}

    /// <summary>
    /// Method for logging in, meant to be called from a button.
    /// </summary>
    public void Login()
    {
        if (loginCreateInputFields[0].text != "" && loginCreateInputFields[1].text != "") // Are the login input fields not empty?
        {
            if (DBManager.Instance.LoginToExistingUser(loginCreateInputFields[0].text, loginCreateInputFields[1].text)) // Did the username and password match a profile in the database?
            {
                print("Login succesfull");
                toaster.ShowToast("Login succesfull");
                startMenuPanel.gameObject.SetActive(false);
                UIController.Instance.LoadScene("Main");
            }
            else
            {
                print("Login unsuccesfull");
                toaster.ShowToast("Login unsuccesfull");
            }
        }
        #region DEBUG
        else // only for debugging
        {
            print("Debug Login");
            toaster.ShowToast("Debug Login");
            startMenuPanel.gameObject.SetActive(false);
            UIController.Instance.LoadScene("Main");
        }
        #endregion
    }

    /// <summary>
    /// Method for creating a user, meant to be called from a button.
    /// </summary>
    public void CreateUser()
    {
        if (loginCreateInputFields[2].text != "" && loginCreateInputFields[3].text != "") // Are the create user input fields not empty?
        {
            if (!DBManager.Instance.ExistingUser(loginCreateInputFields[2].text)) // Does the profile already exist?
            {
                DBManager.Instance.InsertNewUser(loginCreateInputFields[2].text, loginCreateInputFields[3].text); // Insert the new user into the database.
                print("Created user");

                createUserPanel.gameObject.SetActive(false);
                startMenuPanel.gameObject.SetActive(true);

                toaster.ShowToast("Created user");
            }
            else if (DBManager.Instance.ExistingUser(loginCreateInputFields[2].text)) // If the profile already exist.
            {
                print("could not create user");
                toaster.ShowToast("could not create user");
            }
        }
    }
}
