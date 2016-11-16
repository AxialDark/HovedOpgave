using UnityEngine;
using System.Collections;
using System;

public class WorldMap : MonoBehaviour
{

    [SerializeField]
    private Settings settings;

    private RoadFactory roadFac;
    private BuildingFactory buildFac;
    private TileManager tileMan;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        roadFac = GetComponentInChildren<RoadFactory>();
        buildFac = GetComponentInChildren<BuildingFactory>();
        tileMan = GetComponent<TileManager>();

        tileMan.Initialize(buildFac, roadFac, settings);
        //StartCoroutine(Init());
    }

    /// <summary>
    /// Coroutines used on devices with GPS to initialize the map
    /// </summary>
    /// <returns></returns>
    private IEnumerator Init()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;


        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            LocationInfo locData = Input.location.lastData;

            settings.latitude = locData.latitude;
            settings.longtitude = locData.longitude;
            tileMan.Initialize(buildFac, roadFac, settings);
        }
    }

    /// <summary>
    /// Settings for the map
    /// </summary>
    [Serializable]
    public class Settings
    {
        [SerializeField]
        public float latitude = 56.410394f;
        [SerializeField]
        public float longtitude = 10.886543f;
        [SerializeField]
        public int detailLevel = 16;
        [SerializeField]
        public int range = 3;
        [SerializeField]
        public bool loadImages = false;
    }
}