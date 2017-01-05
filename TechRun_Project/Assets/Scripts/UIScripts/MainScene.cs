using UnityEngine;
using System.Collections;

/// <summary>
/// Shows or hides the main menu objects
/// Based on scene is active
/// </summary>
public class MainScene : MonoBehaviour {

    /// <summary>
    /// Unity build in method
    /// When the script is enabled
    /// </summary>
	void OnEnable()
    {
        UIController.Instance.pnlInGameMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// Unity build in method
    /// When the script is disabled
    /// </summary>
    void OnDiable()
    {
        UIController.Instance.pnlInGameMenu.gameObject.SetActive(false);
    }
}
