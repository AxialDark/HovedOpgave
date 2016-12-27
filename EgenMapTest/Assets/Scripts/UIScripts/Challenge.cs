using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contains necesary information for a Challenge
/// </summary>
public class Challenge : MonoBehaviour
{
    /* CHALLENGE FORMAT
    First Vector: Start location
    Remaining Vectors: Via Points

    End point equals start point
    */

    public List<Vector2> routePoints = new List<Vector2>()
    {
        new Vector2(56.407051f, 10.876623f),
        new Vector2(56.407210f, 10.878358f),
        new Vector2(56.408539f, 10.877934f),
        new Vector2(56.408435f, 10.875961f),
    };

    public RouteLength routelength = RouteLength.SHORT;
}
