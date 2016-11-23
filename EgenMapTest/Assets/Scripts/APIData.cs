using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class APIData
{
    private int totalTimeInSeconds;
    private float distanceOfRoute;
    private Vector2[] startAndEndOfRoute;
    private List<Vector2> routeLatLongs;


    public int TotalTimeInSeconds
    {
        get
        {
            return totalTimeInSeconds;
        }
    }
    public float DistanceOfRoute
    {
        get
        {
            return distanceOfRoute;
        }
    }
    public Vector2[] StartAndEndOfRoute
    {
        get
        {
            return startAndEndOfRoute;
        }
    }
    public List<Vector2> RouteLatLongs
    {
        get
        {
            return routeLatLongs;
        }
    }


    public APIData(string _timeString, string _distanceString, List<string> _routePointList)
    {
        totalTimeInSeconds = ConvertTimeStringToInSeconds(_timeString);
        distanceOfRoute = ConvertDistanceStringToMeters(_distanceString);

        SetRoutePointsFromData(_routePointList);
    }

    private int ConvertTimeStringToInSeconds(string _timeString)
    {
        _timeString = _timeString.Trim();
        _timeString = _timeString.Replace("<xls:TotalTime>", "");
        _timeString = _timeString.Replace("</xls:TotalTime>", "");

        _timeString = _timeString.Remove(0, 2);

        string[] splitTime = _timeString.Split('H', 'M', 'S');

        int count = splitTime.Length - 1;

        for (int i = 0; i < count; i++)
        {
            totalTimeInSeconds += ((((count - 1) - i) * 60) == 0) ? int.Parse(splitTime[i]) : int.Parse(splitTime[i]) * (((count - 1) - i) * 60);
        }

        return totalTimeInSeconds;
    }

    private float ConvertDistanceStringToMeters(string _distanceString)
    {
        _distanceString = _distanceString.Trim();
        _distanceString = _distanceString.Replace("<xls:TotalDistance uom=\"M\" value=\"", "");
        _distanceString = _distanceString.Replace("\"/>", "");

        return float.Parse(_distanceString, CultureInfo.InvariantCulture);
    }

    private void SetRoutePointsFromData(List<string> _routePointList)
    {
        startAndEndOfRoute = new Vector2[2];
        routeLatLongs = new List<Vector2>();
        for (int i = 0; i < 2; i++)
        {
            string[] splitCoord = _routePointList[i].Split(' ');

            Vector2 posCoord = new Vector2(float.Parse(splitCoord[1], CultureInfo.InvariantCulture), float.Parse(splitCoord[0], CultureInfo.InvariantCulture));

            startAndEndOfRoute[i] = posCoord;
        }

        for (int i = 2; i < _routePointList.Count; i++)
        {
            string[] splitCoord = _routePointList[i].Split(' ');

            Vector2 posCoord = new Vector2(float.Parse(splitCoord[1], CultureInfo.InvariantCulture), float.Parse(splitCoord[0], CultureInfo.InvariantCulture));

            routeLatLongs.Add(posCoord);
        }
    }
}

