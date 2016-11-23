﻿using UnityEngine;
using System.Collections;

public class WorldView : MonoBehaviour {

    private WebCamTexture view = null;

	// Use this for initialization
	void Start () 
    {
        view = new WebCamTexture();

        gameObject.GetComponent<Renderer>().material.mainTexture = view;

        view.Play();
	}

    void Update()
    {
        Camera cam = Camera.main;

        float pos = (cam.farClipPlane - 5.0f);

        transform.position = cam.transform.position + cam.transform.forward * pos;
        //transform.LookAt(cam.transform);
        //transform.Rotate(90.0f, 0.0f, 0.0f);

        float h = (Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) / 10.0f;

        //transform.localScale = new Vector3(h * cam.aspect, 1.0f, h);
        transform.localScale = new Vector3(h, 1.0f, h * cam.aspect);
    }
}
