using UnityEngine;
using System.Collections;

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
