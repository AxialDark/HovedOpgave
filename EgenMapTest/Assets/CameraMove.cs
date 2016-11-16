using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
	    if (Input.touchCount == 1 && Application.platform == RuntimePlatform.Android)
        {
            Touch touch1 = Input.GetTouch(0);

            if (touch1.phase == TouchPhase.Moved)
            {
                Vector3 newPos = new Vector3(-touch1.deltaPosition.x, 0, -touch1.deltaPosition.y) * Time.deltaTime * 150;

                transform.position += newPos;
            }
        }
	}
}
