﻿using UnityEngine;
using System.Collections;
using System.Diagnostics;

/// <summary>
/// Handles the points given for a route
/// </summary>
public class PointManager : MonoBehaviour
{
    #region Singleton
    private static PointManager instance;

    /// <summary>
    /// The static instance of the PointManager Class, used for singleton purposes.
    /// </summary>
    public static PointManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("PointManager").AddComponent<PointManager>();
            }
            return instance;
        }
    }
    #endregion

    #region Fields
    private int totalPoint;
    private int routePoint;
    private Stopwatch timer;
    private long millisecond;
    private int second, minute, hour;
    #endregion

    /// <summary>
    /// Unity Method, called in the beginning, before Start().
    /// </summary>
    private void Awake()
    {
        if (GameObject.FindObjectsOfType<PointManager>().Length > 1) //Makes sure that no doublicates of this class is created, because of DontDestroyOnLoad().
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Instantiates the Stopwatch, while also starting the timer.
    /// </summary>
    public void StartRouteTimer()
    {
        timer = new Stopwatch();
        timer.Start();
    }

    /// <summary>
    /// Adds a given amout of points to the totalPoint.
    /// </summary>
    /// <param name="_point">Points to be added</param>
    public void AddPoints(int _point)
    {
        totalPoint += _point;
    }

    /// <summary>
    /// Calculates and returns the final score for the route.
    /// </summary>
    /// <param name="_routeLengthInMeters">The routes length in meters.</param>
    /// <returns>Final score for route</returns>
    public int CalcEndScore(int _routeLengthInMeters)
    {
        int elapsedTimeInMinutes = (int)(timer.ElapsedMilliseconds / 60000);

        if (elapsedTimeInMinutes >= 1) routePoint = _routeLengthInMeters / elapsedTimeInMinutes;
        totalPoint += routePoint;

        return totalPoint;
    }

    /// <summary>
    /// Creates the text for the end route panel.
    /// </summary>
    /// <param name="_routeLengthInMeters">Length of the route in meters</param>
    public void MakeEndStats(int _routeLengthInMeters)
    {
        string endStatsText =
            "Time: " + TimeToTimer() + "\n" +
            "Route length: " + _routeLengthInMeters + " meters\n" +
            "Total points: " + CalcEndScore(_routeLengthInMeters) + "\n" +
            "Points from games: " + (totalPoint - routePoint);

        UIController.Instance.ChangeEndStatsText(endStatsText);
    }

    /// <summary>
    /// Resets the timer, as well as the scores.
    /// </summary>
    public void Reset()
    {
        timer.Reset();
        totalPoint = 0;
        routePoint = 0;
    }

    /// <summary>
    /// Returns the elapsed time as a string, formatted in hours, minutes and seconds
    /// </summary>
    /// <returns>Elapsed time</returns>
    public string TimeToTimer()
    {
        if (timer != null)
        {
            millisecond = timer.ElapsedMilliseconds;

            second = (int)(millisecond / 1000);
            minute = Mathf.FloorToInt((float)second / 60.0f);
            second = second % 60;
            hour = Mathf.FloorToInt((float)minute / 60.0f);
            minute = minute % 60;
        }

        return hour + " : " + minute + " : " + second;
    }
}
