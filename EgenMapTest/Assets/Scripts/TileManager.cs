using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Helpers;
using UniRx;
using System;

/// <summary>
/// Super class
/// Manages all tiles.
/// </summary>
public class TileManager : MonoBehaviour
{ //Is gonna be a superclass later

    private readonly string mapzenURL = "https://tile.mapzen.com/mapzen/vector/v1/{0}/{1}/{2}/{3}.{4}?api_key={5}"; //Changed from tut
    private readonly string mapzenKey = "mapzen-ncia6gL";
    private readonly string mapzenLayer = "buildings,roads";
    private readonly string mapzenFormat = "json";

    protected RoadFactory roadFac;
    protected BuildingFactory buildFac;
    protected Transform mapParent;

    protected bool loadImages;
    protected int zoom = 16; //Detail level
    protected int range = 1;
    protected Dictionary<Vector2, Tile> tiles;
    protected Vector2 centerTMS; //TMS (Tile Map Service)
    protected Vector2 centerInMercator; //This is distance (meter) in mercator


    /// <summary>
    /// Initialize fields
    /// </summary>
    /// <param name="_buildFac">BuildingFactory class to build building on tiles</param>
    /// <param name="_roadFac">RoadFactory class to build roads on tiles</param>
    /// <param name="_settings">The settings from the WorldMap class</param>
    public virtual void Initialize(BuildingFactory _buildFac, RoadFactory _roadFac, WorldMap.Settings _settings)
    {
        Vector2 vector = GM.LatLonToMeters(_settings.latitude, _settings.longtitude); //Converts latitude and longtitude to xy cordinates in meters
        Vector2 tile = GM.MetersToTile(vector, _settings.detailLevel); //Gives a tile based on xy cordinates

        mapParent = new GameObject("Map").transform;
        mapParent.SetParent(transform, false);

        roadFac = _roadFac;
        buildFac = _buildFac;
        tiles = new Dictionary<Vector2, Tile>();
        centerTMS = tile;
        zoom = _settings.detailLevel;
        centerInMercator = GM.TileBounds(centerTMS, zoom).center;
        range = _settings.range;
        loadImages = _settings.loadImages;

        //Init the User Marker
        User.Instance.Initialize(new Vector2(_settings.latitude, _settings.longtitude), centerInMercator, mapParent);

        LoadTiles(centerTMS, centerInMercator);
    }

    /// <summary>
    /// Loads tiles in range of center tile
    /// </summary>
    /// <param name="_tileTMS">The center tile in world space</param>
    /// <param name="_centerInMercator">The center in x/y meters</param>
    protected void LoadTiles(Vector2 _tileTMS, Vector2 _centerInMercator)
    {
        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++) //Runs throught the range around the tile
            {
                Vector2 vec = new Vector2(_tileTMS.x + i, _tileTMS.y + j); //Neighbour tile
                if (tiles.ContainsKey(vec)) //If the tile already has been found, look again
                    continue;
                StartCoroutine(CreateTile(vec, _centerInMercator)); //Starts a coroutine to create the tile
            }
        }
    }

    /// <summary>
    /// Creates a tile
    /// </summary>
    /// <param name="_tileTMS">The new Tiles vector location</param>
    /// <param name="_centerInMercator">The center in x/y meters</param>
    /// <returns></returns>
    protected virtual IEnumerator CreateTile(Vector2 _tileTMS, Vector2 _centerInMercator)
    {
        Rect rect = GM.TileBounds(_tileTMS, zoom); //The new Tile bounds
        Tile tile = new GameObject("Tile " + _tileTMS.x + "-" + _tileTMS.y).AddComponent<Tile>().Initialize(buildFac ,roadFac, //Creates the tile with the settings
            new Tile.Settings()
            {
                Zoom = zoom,
                TileTMS = _tileTMS,
                TileCenter = rect.center,
                LoadImages = loadImages
            });

        tiles.Add(_tileTMS, tile); //Adds the tile to the dictionary
        tile.transform.SetParent(mapParent, true); //Sets the map parent as the tile's paren, but the tile keeps it's position in the world
        tile.transform.position = (rect.center - centerInMercator).ToVector3xz(); //Moves the tile to the right position, and makes the y coordinate the z coordinate
        LoadTile(_tileTMS, tile);

        yield return null;
    }

    /// <summary>
    /// Loads tile based on mapzen API data
    /// </summary>
    /// <param name="_tileTMS">The new Tiles vector location</param>
    /// <param name="_tile">The tile that needs to be loaded</param>
    private void LoadTile(Vector2 _tileTMS, Tile _tile)
    {
        string url = string.Format(mapzenURL, mapzenLayer, zoom, _tileTMS.x, _tileTMS.y, mapzenFormat, mapzenKey); //Formats the mapzen url with parameters
        ObservableWWW.Get(url) //Third party code, meant for making task threaded
            .Subscribe(
            _tile.ConstructTile, //succes
            FailToGetDataFromAPI); //Error
    }

    /// <summary>
    /// Error handling method when can't get data from mapzen API
    /// </summary>
    /// <param name="ex"></param>
    private void FailToGetDataFromAPI(Exception ex)
    {
        ErrorPanel.Instance.ShowError("Failed to load map",
            "We were unable to load the map or part of the map.\nPlease check your internet connection and try again", ErrorType.COULD_NOT_LOAD_MAP);

        Debug.Log("Error fetching -> " + ex);
    }
}