using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the movement of camera on User
/// </summary>
public class CameraMove : MonoBehaviour {
    
    /// <summary>
    /// Unity Method.
    /// Runs after the regular Update method.
    /// </summary>
    private void LateUpdate()
    {
        gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
