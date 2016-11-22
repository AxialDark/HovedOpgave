using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugRouting : MonoBehaviour {

    public List<Vector2> debuglatLongs;
    public WorldMap.Settings settings;

    public void Initialize(WorldMap.Settings _settings)
    {
        settings = _settings;

        debuglatLongs = new List<Vector2>();

        //Add via points
        //debuglatLongs.Add(new Vector2(56.407210f, 10.878358f));
        debuglatLongs.Add(new Vector2(56.408539f, 10.877934f));
        //debuglatLongs.Add(new Vector2(56.408610f, 10.876132f));
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.R))
        {
            print("Routing");
            GameObject.Find("RouteManager").GetComponent<RouteManager>().InitiateRouteGeneration(new Vector2(settings.latitude, settings.longtitude), debuglatLongs, settings);
        }
	}
}
