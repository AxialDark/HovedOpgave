using UnityEngine;
using System.Collections;

/// <summary>
/// Makes sure the Google VR Plugin does not cause an exception when switching to camera this is on.
/// </summary>
public class GvrEx : MonoBehaviour {

    /// <summary>
    /// Unity method
    /// Called when attached game object gets enabled
    /// </summary>
    void OnEnable()
    {
        if (GetComponent<GvrHead>()) { Destroy(gameObject.GetComponent<GvrHead>()); }
        if (GetComponent<StereoController>()) { Destroy(gameObject.GetComponent<StereoController>()); }
    }
}
