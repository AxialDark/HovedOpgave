using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the create user panel Inputfields
/// </summary>
public class Panel_CreateUser : MonoBehaviour {

    /// <summary>
    /// Unity build in method
    /// When script is enabled
    /// </summary>
    private void OnEnable()
    {
        InputField[] children = GetComponentsInChildren<InputField>();

        for (int i = 0; i < children.Length; i++)
        {
            children[i].gameObject.GetComponent<InputField>().text = "";
        }
    }
}
