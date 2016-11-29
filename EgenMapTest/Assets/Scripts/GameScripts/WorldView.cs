using UnityEngine;
using System.Collections;

/// <summary>
/// Script from Google Vr, with minor alterations, allows us to get camerafeed, and apply it to a plane.
/// </summary>
public class WorldView : MonoBehaviour {

    private WebCamTexture view = null;

	/// <summary>
    /// Unity method, runs in the beginning
    /// </summary>
	private void Start () 
    {
        view = new WebCamTexture();

        gameObject.GetComponent<Renderer>().material.mainTexture = view;

        view.Play();
	}

    /// <summary>
    /// Unity method, runs once every frame
    /// </summary>
    private void Update()
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

    /// <summary>
    /// Unity method. Called when attached game object is destroyed
    /// </summary>
    void OnDestroy()
    {
        view.Stop();
    }
}
