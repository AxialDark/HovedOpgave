using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// Class converting and containing data from route API
/// </summary>
public class APIData
{
    private int totalTimeInSeconds;
    private float distanceOfRoute;
    private Vector2[] startAndEndOfRoute;
    private List<Vector2> routeLatLongs;

    /// <summary>
    /// The total time of the route in seconds
    /// </summary>
    public int TotalTimeInSeconds
    {
        get
        {
            return totalTimeInSeconds;
        }
    }
    /// <summary>
    /// The distance of the route in meters
    /// </summary>
    public float DistanceOfRoute
    {
        get
        {
            return distanceOfRoute;
        }
    }
    /// <summary>
    /// Array of start and end latitude and longtitude of route
    /// </summary>
    public Vector2[] StartAndEndOfRoute
    {
        get
        {
            return startAndEndOfRoute;
        }
    }
    /// <summary>
    /// All latitude and longtitudes coordinates of the route
    /// </summary>
    public List<Vector2> RouteLatLongs
    {
        get
        {
            return routeLatLongs;
        }
    }

    /// <summary>
    /// Constructor for APIData class
    /// </summary>
    /// <param name="_timeString">The time string from the API data</param>
    /// <param name="_distanceString">The distance string from the API data</param>
    /// <param name="_routePointList">All latitude and longtitude points from APIDataExtractor</param>
    public APIData(string _timeString, string _distanceString, List<string> _routePointList)
    {
        totalTimeInSeconds = ConvertTimeStringToInSeconds(_timeString);
        distanceOfRoute = ConvertDistanceStringToMeters(_distanceString);

        SetRoutePointsFromData(_routePointList);
    }

    /// <summary>
    /// Converts time string to int variable
    /// </summary>
    /// <param name="_timeString">The time string data</param>
    /// <returns>The time in seconds</returns>
    private int ConvertTimeStringToInSeconds(string _timeString)
    {
        _timeString = _timeString.Trim(); //Removes all whitespaces
        _timeString = _timeString.Replace("<xls:TotalTime>", ""); //Removes start data tag
        _timeString = _timeString.Replace("</xls:TotalTime>", ""); //Removes end data tag

        _timeString = _timeString.Remove(0, 2); //Removes the 2 first charectors from string ("PT")

        string[] splitTime = _timeString.Split('H', 'M', 'S'); //Split the time into Hours, Minutes and Seconds

        int count = splitTime.Length - 1; //Removes last empty string from string.Split()

        for (int i = 0; i < count; i++) //Runs through the split time
        {
            //Hours and minutes gets multiplied by a magnitude of 60 times their place in array
            //Seconds gets parsed directly
            totalTimeInSeconds += ((((count - 1) - i) * 60) == 0) ? int.Parse(splitTime[i]) : int.Parse(splitTime[i]) * (((count - 1) - i) * 60);
        }

        return totalTimeInSeconds;
    }

    /// <summary>
    /// Converts distance string to float variable
    /// </summary>
    /// <param name="_distanceString">The distance string data</param>
    /// <returns>The distance in meters</returns>
    private float ConvertDistanceStringToMeters(string _distanceString)
    {
        _distanceString = _distanceString.Trim(); //Removes whitespaces
        _distanceString = _distanceString.Replace("<xls:TotalDistance uom=\"M\" value=\"", ""); //Removes the start tag from data
        _distanceString = _distanceString.Replace("\"/>", ""); //Removes the end tag from data

        return float.Parse(_distanceString, CultureInfo.InvariantCulture); //Converts the value to a float, and keep as many decimals
    }

    /// <summary>
    /// Puts the latitude and longtitude points from data, into a list
    /// </summary>
    /// <param name="_routePointList">List of longtitude and latitude points from data</param>
    private void SetRoutePointsFromData(List<string> _routePointList)
    {
        startAndEndOfRoute = new Vector2[2]; //Instantiates an array with 2 spaces
        routeLatLongs = new List<Vector2>(); //Instantiates a list

        for (int i = 0; i < 2; i++) //Runs through the first 2 values from the list (start and end of route)
        {
            string[] splitCoord = _routePointList[i].Split(' '); //Split it in a whitespace

            //From data it is formatet longtitude and latitude, and we need it as latitude and longtitude
            Vector2 posCoord = new Vector2(float.Parse(splitCoord[1], CultureInfo.InvariantCulture), float.Parse(splitCoord[0], CultureInfo.InvariantCulture));

            startAndEndOfRoute[i] = posCoord; //Add the point to array
        }

        for (int i = 2; i < _routePointList.Count; i++) //Runs through all the other values
        {
            string[] splitCoord = _routePointList[i].Split(' '); //Split it in a whitespace

            //From data it is formatet longtitude and latitude, and we need it as latitude and longtitude
            Vector2 posCoord = new Vector2(float.Parse(splitCoord[1], CultureInfo.InvariantCulture), float.Parse(splitCoord[0], CultureInfo.InvariantCulture));

            routeLatLongs.Add(posCoord); //Add lat/long to list
        }
    }
}

