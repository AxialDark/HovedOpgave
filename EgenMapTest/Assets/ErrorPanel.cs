using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Enumeration depicting the type of error that can occur
/// </summary>
public enum ErrorType {
    NONE,
    COULD_NOT_FIND_ROUTE,
    COULD_NOT_LOAD_MAP,
    COULD_NOT_LOAD_ROUTE,
    GPS_STATE_NOT_RUNNING }

/// <summary>
/// Class handling the error window show to the user when using the application
/// </summary>
public class ErrorPanel : MonoBehaviour {

    private static ErrorPanel instance;    
    private Text[] errorPanelTexts;
    private ErrorType error;

    /// <summary>
    /// Static instance of the ErrorPanel GameObject
    /// </summary>
    public static ErrorPanel Instance
    {
        get
        { 
            return instance;
        }
    }

    /// <summary>
    /// Unity Start Method
    /// </summary>
    private void Start()
    {
        instance = this;
        errorPanelTexts = GetComponentsInChildren<Text>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Show the Error Panel
    /// </summary>
    /// <param name="_subTitle">The subtitle for the error</param>
    /// <param name="_info">The information to the user</param>
    /// <param name="_type">The type of error</param>
    public void ShowError(string _subTitle, string _info, ErrorType _type)
    {
        errorPanelTexts[1].text = _subTitle;
        errorPanelTexts[2].text = _info;
        error = _type;

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the Error Panel
    /// </summary>
    private void HideError()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// OnClick Method for the Ok button
    /// </summary>
    public void OnOkClick()
    {



        ResetErrorText();
        HideError();
        error = ErrorType.NONE;
    }

    /// <summary>
    /// Resets the error tekst to standard
    /// </summary>
    private void ResetErrorText()
    {
        errorPanelTexts[0].text = "Sorry... something went wrong";
        errorPanelTexts[1].text = "Error - ";
        errorPanelTexts[2].text = "Error";
    }
}