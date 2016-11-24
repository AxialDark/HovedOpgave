using UnityEngine;
using System.Collections;

public class GameLocation : MonoBehaviour
{
    private bool used = false;
    private GameType gameType;

    public bool Used
    {
        get
        {
            return used;
        }
    }
    public GameType Gametype
    {
        get
        {
            return gameType;
        }
    }

    public GameLocation Initialize()
    {
        //TODO: set gameType as a random enum GameType
        gameType = GameType.BASKETBALL;

        return this;
    }
}
