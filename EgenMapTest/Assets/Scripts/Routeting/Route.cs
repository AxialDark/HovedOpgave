using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Helpers;
using UniRx;
using System.Linq;

public class Route : MonoBehaviour {

    private readonly string distanceFormat = "M";
    private readonly string apiUrl = "http://openls.geog.uni-heidelberg.de/route?api_key=ee0b8233adff52ce9fd6afc2a2859a28&start={0}&end={1}&via={2}&lang={3}&distunit={4}&routepref={5}&weighting={6}&avoidAreas=&useTMC=false&noMotorways=false&noTollways=false&noUnpavedroads=false&noSteps=false&noFerries=false&instructions=false";
    private readonly string transportType = "Pedestrian";
    private readonly string routingLanguage = "en";
    private readonly string routeWeight = "Recommended";

    private int distance;
    private List<Vector2> viaLatLongs;
    private List<Vector2> routeLatLongs = new List<Vector2>();
    private List<Vector2> routeInMercCoords = new List<Vector2>();
    private bool dataLoaded;
    private TimeSpan estimatedTime;
    private int zoom = 16;


    public bool DataLoaded { get { return dataLoaded; } }
    public List<Vector2> RouteInMercCoords { get { return routeInMercCoords; } }
    public int Distance { get { return distance; } }
    public TimeSpan EstimatedTime { get { return estimatedTime; } }


    private static char[] numberArray = new char[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };


    public Route Initialize(Vector2 _startPos, List<Vector2> _via, int _zoom)
    {
        viaLatLongs = _via;
        zoom = _zoom;
        dataLoaded = false;
        distance = 0;

        LoadAPIData(_startPos, _via);

        return this;
    }

    public void LoadAPIData(Vector2 _startPos, List<Vector2> _via)
    {
        string startEnd = _startPos.y + "," + _startPos.x;

        string viaFormatedString = "";

        for (int i = 0; i < _via.Count; i++)
        {
            viaFormatedString += _via[i].y + "," + _via[i].x + "%20";
        }

        viaFormatedString = viaFormatedString.Remove(viaFormatedString.Length - 3);

        string url = string.Format(apiUrl, startEnd, startEnd, viaFormatedString, routingLanguage, distanceFormat, transportType, routeWeight);
        ObservableWWW.Get(url) //Third party code, meant for making task threaded
            .Subscribe(
            ConvertAPIData, //succes
            exp => Debug.Log("Error fetching -> " + url)); //Error
        
        
        
        //ConvertAPIData(_startPos);
    }

    private void ConvertAPIData(string _text)
    {
        _text = _text.Trim('\n');

        _text = _text.Replace("<gml:pos>", ";");
        _text = _text.Replace("</gml:pos>", ";");

        string[] splitResult = _text.Split(';');
        List<string> tal = new List<string>();

        for (int i = 0; i < splitResult.Length; i++)
        {
            if (numberArray.Contains(splitResult[i][0]))
            {
                tal.Add(splitResult[i]);
            }
        }

        tal.RemoveAt(0); //Forgot to format first two values :)
        tal.RemoveAt(0); //Forgot to format first two values :)

        for (int i = 0; i < tal.Count; i++) //Format and add all of the routes latitudes and longtitude
        {
            string[] longLat = tal[i].Split(' ');

            routeLatLongs.Add(new Vector2(float.Parse(longLat[1]), float.Parse(longLat[0])));
        }

        //Find the tile the player stands in
        Vector2 vector = GM.LatLonToMeters(routeLatLongs[0].x, routeLatLongs[0].y);
        Vector2 tile = GM.MetersToTile(vector, zoom);

        Vector2 centerInMercator = GM.TileBounds(tile, zoom).center; //Finds the center of the tile

        //Finds the position reletive to the tile the player stands on
        for (int i = 0; i < routeLatLongs.Count; i++)
        {
            vector = GM.LatLonToMeters(routeLatLongs[i].x, routeLatLongs[i].y);

            routeInMercCoords.Add((vector - centerInMercator)); //In mercator coordinates
        }

        dataLoaded = true;
    }

    //private void ConvertAPIData(Vector2 _startPos)
    //{
    //    //Start pos
    //    routeLatLongs.Add(_startPos); //Add start position as first in list

    //    for (int i = 0; i < viaLatLongs.Count; i++) //Add all latitudes and longtitudes to list
    //    {
    //        routeLatLongs.Add(viaLatLongs[i]);
    //    }

    //    //Route end pos
    //    routeLatLongs.Add(_startPos); //Add start position as last in list 

    //    //Find the tile the player stands in
    //    Vector2 vector = GM.LatLonToMeters(_startPos.x, _startPos.y);
    //    Vector2 tile = GM.MetersToTile(vector, zoom);

    //    Vector2 centerInMercator = GM.TileBounds(tile, zoom).center; //Finds the center of the tile

    //    //Finds the position reletive to the tile the player stands on
    //    for (int i = 0; i < routeLatLongs.Count; i++)
    //    {
    //        vector = GM.LatLonToMeters(routeLatLongs[i].x, routeLatLongs[i].y);

    //        routeInMercCoords.Add((vector - centerInMercator)); //In mercator coordinates
    //    }

    //    for (int i = 1; i < routeInMercCoords.Count; i++)
    //    {
    //        float diffx = Mathf.Abs(routeInMercCoords[i - 1].x - routeInMercCoords[i].x);
    //        float diffy = Mathf.Abs(routeInMercCoords[i - 1].y - routeInMercCoords[i].y);
    //        distance += (int)Mathf.Round(Mathf.Sqrt(Mathf.Pow(diffx, 2) + Mathf.Pow(diffy, 2)));
    //    }



    //    dataLoaded = true;
    //}
}