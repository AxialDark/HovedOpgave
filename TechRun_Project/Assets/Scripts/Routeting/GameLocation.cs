using UnityEngine;
using System.Collections;

/// <summary>
/// Handles very speified game location behavior
/// </summary>
public class GameLocation : MonoBehaviour
{
    private GameType gameType;

    /// <summary>
    /// A game locations type of game (eg. Basketball)
    /// </summary>
    public GameType Gametype { get { return gameType; } }
    /// <summary>
    /// The point the GameLocations sits on
    /// </summary>
    public GameObject RoutePoint { get; set; }

    /// <summary>
    /// Initialize a game location
    /// </summary>
    /// <returns>The game location</returns>
    public GameLocation Initialize()
    {
        //TODO: set gameType as a random enum GameType
        gameType = GameType.BASKETBALL;

        return this;
    }

}
