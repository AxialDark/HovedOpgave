using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RouteLength { SHORT = 2, MIDDLE = 5, LONG = 10 }

public class DebugRouting : MonoBehaviour
{

    public static List<Vector2> debuglatLongs;
    public static WorldMap.Settings settings;
    private float randomOffset;

    public void Initialize(WorldMap.Settings _settings)
    {
        settings = _settings;
        debuglatLongs = new List<Vector2>();
        randomOffset = Random.Range(0.00010f, 0.00050f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            print("Hardcode Routing...");
            debuglatLongs.Clear();
            //Add via points
            //debuglatLongs.Add(new Vector2(56.407210f, 10.878358f));
            debuglatLongs.Add(new Vector2(56.408539f, 10.877934f));
            debuglatLongs.Add(new Vector2(56.408435f, 10.875961f));
            GameObject.Find("RouteManager").GetComponent<RouteManager>().InitiateRouteGeneration(new Vector2(settings.latitude, settings.longtitude), debuglatLongs, settings);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            RandomRoute();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            RouteManager.EndRoute();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            print("Real Random Routing...");
            debuglatLongs.Clear();
            RandomRouting(RouteLength.LONG);
            GameObject.Find("RouteManager").GetComponent<RouteManager>().InitiateRouteGeneration(new Vector2(settings.latitude, settings.longtitude), debuglatLongs, settings);
        }
    }

    private float GenerateRandomFloat()
    {
        return 0f;
    }

    private void RandomRouting(RouteLength _length)
    {
        //FourPoints(_length);
        StarPoints(_length);
    }

    private void FourPoints(RouteLength _length)
    {
        float rnd = (float)_length * 3 * 0.0015f;//Random.Range((float)length * 2 * 0.001f, (float)length * 4 * 0.001f);

        Vector2 dir = Vector2.zero;

        do
        {
            dir = Vector2.up;
            //dir = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
        } while (dir == Vector2.zero);
        Vector2 otherDir = new Vector2(dir.y, dir.x);

        //THE MIDDLE
        Vector2 middlePoint = new Vector2(settings.latitude + ((rnd / 4) * dir.y), settings.longtitude + ((rnd / 2) * dir.x));

        //LEFT
        Vector2 viaPoint = new Vector2(middlePoint.x + ((rnd / 4) * otherDir.y), middlePoint.y + ((rnd / 2) * otherDir.x));
        debuglatLongs.Add(viaPoint);

        //UP
        viaPoint = new Vector2(middlePoint.x + ((rnd / 4) * dir.y), middlePoint.y + ((rnd / 2) * dir.x));
        debuglatLongs.Add(viaPoint);

        //RIGHT
        otherDir *= -1;
        viaPoint = new Vector2(middlePoint.x + ((rnd / 4) * otherDir.y), middlePoint.y + ((rnd / 2) * otherDir.x));

        debuglatLongs.Add(viaPoint);
    }

    private void StarPoints(RouteLength _length)
    {
        float bigRnd = (float)_length * 3f * 0.001f;
        float smallRnd = (float)_length * 1f * 0.0016f;


        Vector2 dir = Vector2.zero;

        do
        {
            dir = Vector2.right;
            //dir = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
        } while (dir == Vector2.zero);
        Vector2 otherDir = new Vector2(dir.y, dir.x);

        //MiddlePoint
        Vector2 middlePoint = new Vector2(settings.latitude + ((bigRnd / 4) * dir.y), settings.longtitude + ((bigRnd / 2) * dir.x));

        //Left
        Vector2 left = new Vector2(middlePoint.x + ((bigRnd / 4) * otherDir.y), middlePoint.y + ((bigRnd / 2) * otherDir.x));

        //Top
        Vector2 top = new Vector2(middlePoint.x + ((bigRnd / 4) * dir.y), middlePoint.y + ((bigRnd / 2) * dir.x));

        //Right
        otherDir *= -1;
        Vector2 right = new Vector2(middlePoint.x + ((bigRnd / 4) * otherDir.y), middlePoint.y + ((bigRnd / 2) * otherDir.y));

        //BotLeft
        Vector2 botLeftDir = (new Vector2(settings.latitude - middlePoint.x, settings.longtitude - middlePoint.y) + new Vector2(left.x - middlePoint.x, left.y - middlePoint.y));
        botLeftDir.Normalize();
        Vector2 botLeft = middlePoint + botLeftDir * smallRnd;

        //TopLeft
        Vector2 topLeftDir = (new Vector2(left.x - middlePoint.x, left.y - middlePoint.y) + new Vector2(top.x - middlePoint.x, top.y - middlePoint.y));
        topLeftDir.Normalize();
        Vector2 topLeft = middlePoint + topLeftDir * smallRnd;

        //TopRight
        Vector2 topRightDir = (new Vector2(top.x - middlePoint.x, top.y - middlePoint.y) + new Vector2(right.x - middlePoint.x, right.y - middlePoint.y));
        topRightDir.Normalize();
        Vector2 topRight = middlePoint + topRightDir * smallRnd;

        //BotRight
        Vector2 botRightDir = (new Vector2(right.x - middlePoint.x, right.y - middlePoint.y) + new Vector2(settings.latitude - middlePoint.x, settings.longtitude - middlePoint.y));
        botRightDir.Normalize();
        Vector2 botRight = middlePoint + botRightDir * smallRnd;

        debuglatLongs.Add(botLeft);
        debuglatLongs.Add(left);
        debuglatLongs.Add(topLeft);
        debuglatLongs.Add(top);
        debuglatLongs.Add(topRight);
        debuglatLongs.Add(right);
        debuglatLongs.Add(botRight);
    }
}
