using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Assets.Helpers;

public class RouteManager : MonoBehaviour {

    private Route route;
    private Transform mapParent;

    private static List<GameObject> routeAndPoints = new List<GameObject>();

    public static List<GameObject> RouteAndPoints { get { return routeAndPoints; } }
    
    private void Update()
    {

    }

    public void InitiateRouteGeneration(Vector2 _myLatLong, List<Vector2> _via, WorldMap.Settings _settings)
    {
        print("Initiating Generation");
        if (mapParent == null)
        {
            mapParent = GameObject.Find("Map").transform;
        }

        route = new Route().Initialize(_myLatLong, _via, _settings.detailLevel);

        StartCoroutine(RouteToMap());
    }

    private IEnumerator RouteToMap()
    {
        while (!route.DataLoaded)
        {
            yield return null;
        }

        print("Creating route on map");

        for (int i = 1; i < route.RouteInMercCoords.Count; i++)
        {
            GameObject routePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            routePlane.transform.SetParent(mapParent);

            float distance = Vector2.Distance(route.RouteInMercCoords[i - 1], route.RouteInMercCoords[i]);

            Vector2 diff = new Vector2(route.RouteInMercCoords[i].x - route.RouteInMercCoords[i - 1].x, route.RouteInMercCoords[i].y - route.RouteInMercCoords[i - 1].y);

            Vector3 temp = new Vector3((diff.x / 2) + route.RouteInMercCoords[i - 1].x, 0.2f, (diff.y / 2) + route.RouteInMercCoords[i - 1].y);

            routePlane.transform.position = temp;
            routePlane.name = "Route between: " + i + " - " + (i + 1);
            
            routePlane.transform.localScale = new Vector3(0.25f, 0, distance / 10);

            routePlane.GetComponent<Renderer>().material = Resources.Load<Material>("DebugRoute");

            //DEBUG SPHERES
            GameObject gm = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gm.name = "Route point " + i;
            gm.transform.SetParent(mapParent);
            gm.transform.position = new Vector3(route.RouteInMercCoords[i - 1].x, 10, route.RouteInMercCoords[i - 1].y);
            gm.transform.localScale = new Vector3(10, 10, 10);
            //END DEBUG SPHERES

            routePlane.transform.LookAt(new Vector3(route.RouteInMercCoords[i].x, 0, route.RouteInMercCoords[i].y));

            routeAndPoints.Add(routePlane);
            routeAndPoints.Add(gm);
        }

        print("Distance: " + route.Distance);

    }

    private void EndRoute()
    {
        //Clears the route from game
        foreach (GameObject routeOrPoint in routeAndPoints)
        {
            Destroy(routeOrPoint);
        }

        routeAndPoints.Clear();
    }
}
