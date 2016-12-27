using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Class the handles the overall map
/// Uses other managers to create and handle aspects of the map
/// </summary>
public class WorldMap : MonoBehaviour
{
    [SerializeField]
    private Settings settings;

    private RoadFactory roadFac;
    private BuildingFactory buildFac;
    private TileManager tileMan;
    public static MapColorPalet colorPalet;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        roadFac = GetComponentInChildren<RoadFactory>();
        buildFac = GetComponentInChildren<BuildingFactory>();
        tileMan = GetComponent<TileManager>();

        if (settings.mapColorPalet != colorPalet)
            colorPalet = settings.mapColorPalet;

#if UNITY_EDITOR
        tileMan.Initialize(buildFac, roadFac, settings);
        new GameObject("DebugRouting").AddComponent<DebugRouting>().Initialize(settings);
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(Init());
#endif

        UIController.Instance.Map = this;
    }

    /// <summary>
    /// Initialize the World Map based on location data.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Init()
    {
        if (Input.location.status != LocationServiceStatus.Running) yield return null; //Make sure the GPS is up and running before we start the map generation

          // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude 
                + " " + Input.location.lastData.longitude + " " 
                + Input.location.lastData.altitude + " " 
                + Input.location.lastData.horizontalAccuracy + " " 
                + Input.location.lastData.timestamp);
            LocationInfo locData = Input.location.lastData;

            settings.latitude = locData.latitude;
            settings.longtitude = locData.longitude;
            tileMan.Initialize(buildFac, roadFac, settings);
    }

    /// <summary>
    /// Begins the process of generating a route via the RouteManager class.
    /// Use when generating route with the location of the device.
    /// </summary>
    /// <param name="_routeLength">The desired lenth of the route</param>
    /// <param name="_viaPoints">An optional list of via points. Works as "checkpoints" between start and end points in a route</param>
    public void BeginRouteGeneration(RouteLength _routeLength, List<Vector2> _viaPoints = null)
    {
        RouteManager.Instance.GetComponent<RouteManager>().InitiateRouteGeneration(new Vector2(settings.latitude, settings.longtitude), _viaPoints, settings, _routeLength);
    }

    /// <summary>
    /// Begins the process of generating a route via the RouteManager class.
    /// Use when generating route not based on devices own location (eg. challenge).
    /// </summary>
    /// <param name="_routePoints">A list containing all points of a route</param>
    /// <param name="_routelength">The desired lenth of the route</param>
    public void BeginRouteGeneration(List<Vector2> _routePoints, RouteLength _routeLength)
    {
        List<Vector2> tempViaPoints = _routePoints; //Makes a list we can manipulate

        Vector2 startPoint = _routePoints[0]; //Extracts the start point of route

        tempViaPoints.RemoveAt(0); //Removes the start point making it a list of via points

        RouteManager.Instance.GetComponent<RouteManager>().InitiateRouteGeneration(startPoint, tempViaPoints, settings, _routeLength, true);
    }

    /// <summary>
    /// Changes the Material on all buildings, tiles and roads
    /// </summary>
    /// <param name="_palet">The Color Theme to change to</param>
    public void ChangeColorTheme(MapColorPalet _palet)
    {
        settings.mapColorPalet = _palet;
        colorPalet = _palet;

        foreach (Tile tile in tileMan.AllTiles)
        {
            tile.myRend.material = Resources.Load<Material>("Map Themes/" + settings.mapColorPalet + "/Ground");
        }

        foreach (Building building in buildFac.AllBuildings)
        {
            building.myRendere.material = Resources.Load<Material>("Map Themes/" + settings.mapColorPalet + "/" + building.LanduseKind);
        }

        foreach (RoadPolygon m in roadFac.MyRoads)
        {
            foreach (Renderer rend in m.MyRenderes)
            {
                rend.material = Resources.Load<Material>("Map Themes/" + settings.mapColorPalet + "/Road");
            }
        }
    }


    /// <summary>
    /// Settings for the map
    /// </summary>
    [Serializable]
    public class Settings
    {

        //56.407051, 10.876623
        [SerializeField]
        public float latitude = 56.407051f;//56.410394f;
        [SerializeField]
        public float longtitude = 10.876623f;//10.886543f;
        [SerializeField]
        public int detailLevel = 16;
        [SerializeField]
        public int range = 3;
        [SerializeField]
        public bool loadImages = false;
        [SerializeField]
        public MapColorPalet mapColorPalet = MapColorPalet.DARK;
    }
}