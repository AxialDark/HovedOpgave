﻿using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
    
    /// <summary>
    /// Unity method
    /// Called when attached game object gets enabled
    /// </summary>
    void OnEnable()
    {
        if(GetComponent<GvrHead>()) { Destroy(gameObject.GetComponent<GvrHead>()); }
        if (GetComponent<StereoController>()) { Destroy(gameObject.GetComponent<StereoController>()); }
    }

	// Update is called once per frame
	void Update () {
        //if (Input.touchCount == 1 && Application.platform == RuntimePlatform.Android)
        //   {
        //       Touch touch1 = Input.GetTouch(0);

        //       if (touch1.phase == TouchPhase.Moved)
        //       {
        //           Vector3 newPos = new Vector3(-touch1.deltaPosition.x, 0, -touch1.deltaPosition.y) * Time.deltaTime * 150;

        //           transform.position += newPos;
        //       }
        //   }

        //transform.position = new Vector3(User.Instance.gameObject.transform.position.x, 47.5f, User.Instance.gameObject.transform.position.z);
	}

    private void LateUpdate()
    {
        gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
