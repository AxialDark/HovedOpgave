using UnityEngine;
using System.Collections;

public class MainScene : MonoBehaviour {

	void OnEnable()
    {
        UIController.Instance.pnlInGameMenu.gameObject.SetActive(true);
    }

    void OnDiable()
    {
        UIController.Instance.pnlInGameMenu.gameObject.SetActive(false);
    }
}
