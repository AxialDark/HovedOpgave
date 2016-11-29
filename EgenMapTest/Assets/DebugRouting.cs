using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugRouting : MonoBehaviour {

    public static List<Vector2> debuglatLongs;
    public static WorldMap.Settings settings;
    private float randomOffset;

    public void Initialize(WorldMap.Settings _settings)
    {
        settings = _settings;
        debuglatLongs = new List<Vector2>();
        randomOffset = Random.Range(0.00010f, 0.00050f);
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.H))
        {
            print("Hardcode Routing...");
            //Add via points
            //debuglatLongs.Add(new Vector2(56.407210f, 10.878358f));
            debuglatLongs.Add(new Vector2(56.408539f, 10.877934f));
            debuglatLongs.Add(new Vector2(56.408435f, 10.875961f));
            GameObject.Find("RouteManager").GetComponent<RouteManager>().InitiateRouteGeneration(new Vector2(settings.latitude, settings.longtitude), debuglatLongs, settings);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            RandomRoute();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            RouteManager.EndRoute();
        }
	}

    private float GenerateRandomFloat()
    {
        return 0f;
    }

    public static void RandomRoute()
    {
        print("Random Routing...");
        debuglatLongs.Clear();
        debuglatLongs.Add(new Vector2(settings.latitude + 0.0077f, settings.longtitude));
        debuglatLongs.Add(new Vector2(settings.latitude + 0.0027f, settings.longtitude + 0.0031f));
        GameObject.Find("RouteManager").GetComponent<RouteManager>().InitiateRouteGeneration(new Vector2(settings.latitude, settings.longtitude), debuglatLongs, settings);
    }
}
