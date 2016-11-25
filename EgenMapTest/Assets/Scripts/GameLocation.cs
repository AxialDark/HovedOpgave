using UnityEngine;
using System.Collections;

/// <summary>
/// Handles very speified game location behavior
/// </summary>
public class GameLocation : MonoBehaviour
{
    private bool used = false;
    private GameType gameType;

    /// <summary>
    /// Has the GameLocation been used
    /// </summary>
    public bool Used { get { return used; } }
    /// <summary>
    /// A game locations type of game (eg. Basketball)
    /// </summary>
    public GameType Gametype { get { return gameType; } }

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
