using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Panel_CreateUser : MonoBehaviour {

    void OnEnable()
    {
        InputField[] children = GetComponentsInChildren<InputField>();

        for (int i = 0; i < children.Length; i++)
        {
            children[i].gameObject.GetComponent<InputField>().text = "";
        }
    }
}
