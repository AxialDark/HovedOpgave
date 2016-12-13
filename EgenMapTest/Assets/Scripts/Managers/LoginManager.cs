using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    InputField[] loginCreateInputFields; // 0 = Login username; 1 = Login password; 2 = Create username; 3 = Create password.

    private Toaster toaster;

    // Use this for initialization
	void Awake () 
    {
        toaster = new Toaster();
	}

    public void Login()
    {
        toaster.ShowToast("I work");
        if (loginCreateInputFields[0].text != "" && loginCreateInputFields[1].text != "")
        {
            if (DBManager.Instance.LoginToExistingUser(loginCreateInputFields[0].text, loginCreateInputFields[1].text))
            {
                print("Login succesfull");
                toaster.ShowToast("Login succesfull");
            }
            else
            {
                print("Login unsuccesfull");
                toaster.ShowToast("Login unsuccesfull");
            }
        }
    }

    public void CreateUser()
    {
        if (loginCreateInputFields[2].text != "" && loginCreateInputFields[3].text != "")
        {
            if (!DBManager.Instance.ExistingUser(loginCreateInputFields[2].text))
            {
                DBManager.Instance.InsertNewUser(loginCreateInputFields[2].text, loginCreateInputFields[3].text);
                print("Created user");
                toaster.ShowToast("Created user");
            }
            else if (DBManager.Instance.ExistingUser(loginCreateInputFields[2].text))
            {
                print("could not create user");
                toaster.ShowToast("could not create user");
            }
        }
    }
}
