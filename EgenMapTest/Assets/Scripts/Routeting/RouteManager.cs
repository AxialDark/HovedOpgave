﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Assets.Helpers;

/// <summary>
/// Describes the length of the desired route
/// </summary>
public enum RouteLength { SHORT = 2, MIDDLE = 5, LONG = 10 }

/// <summary>
/// Class to manage the route data generated by the Open Street Map API
/// This includes getting data from the Route class and uses to
/// creates object in the scene visualizing the route to the user
/// </summary>
public class RouteManager : MonoBehaviour
{
    private static RouteManager instance;

    private Route route;
    private bool routeInUse = false;
    private Transform mapParent;

    private List<GameObject> routePlanes = new List<GameObject>();
    private List<GameObject> points = new List<GameObject>();
    private List<GameLocation> gamelocations = new List<GameLocation>();
    private List<Vector2> directions = new List<Vector2>();

    /// <summary>
    /// Singleton of RouteManager
    /// </summary>
    public static RouteManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<RouteManager>();
            }

            return instance;
        }
    }

    public List<GameObject> debugViaPoints = new List<GameObject>();

    /// <summary>
    /// List of game objects generated, in between points, making out the entire route
    /// </summary>
    public List<GameObject> RoutePlanes { get { return routePlanes; } }
    /// <summary>
    /// List of points making out the route based on Via Points given to Open Street Map API
    /// </summary>
    public List<GameObject> Points { get { return points; } }
    /// <summary>
    /// List of game location objects based on point locations and the routes estimated lengh
    /// </summary>
    public List<GameLocation> Gamelocations { get { return gamelocations; } }

    /// <summary>
    /// Unity build-in update method
    /// </summary>
    private void Update()
    {
        //If a route exists and it's current in use
        //but it's empty, it means the route is completed by a user
        if (route != null && routeInUse && routePlanes.Count == 0)
        {
            EndRoute(); //Makes sure the routes and points are cleaned up
            routeInUse = false;

            print("Final points: " + PointManager.Instance.CalcEndScore((int)route.Distance)); //SHOULD BE CHANGED
            PointManager.Instance.Reset();
            route = null;
        }
    }

    /// <summary>
    /// Initiate the generation of one route
    /// </summary>
    /// <param name="_myLatLong">The position of the user</param>
    /// <param name="_via">List of via points API uses to generate route</param>
    /// <param name="_settings">WorldMap settings</param>
    /// <param name="_routeLength">The desired length of the route to generate</param>
    /// <param name="_ignoreRetry">Optinal: Should the route reload from the API of the length doesn't meet the desired lenggth. Set as false as standard</param>
    public void InitiateRouteGeneration(Vector2 _myLatLong, List<Vector2> _via, WorldMap.Settings _settings, RouteLength _routeLength, bool _ignoreRetry = false)
    {
        if (!routeInUse) //Makes the generation of a route impossible if one is already in use.
        {
            directions.Clear(); // Clear the leftover directions
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (!(x == 0 && y == 0))
                        directions.Add(new Vector2(x, y)); // Adds the directions
                }
            }

            print("Initiating Generation");
            if (mapParent == null)
            {
                mapParent = GameObject.Find("Map").transform;
            }

            if (_via == null) //TODO: Handle appropiately. Might happen outside debugging
                _via = RandomRouting(_routeLength, new Vector2(_settings.latitude, _settings.longtitude)); //Creates a random route

            route = new Route().Initialize(_myLatLong, _via, _settings.detailLevel, _routeLength, _ignoreRetry); //Make the route

            StartCoroutine(RouteToMap()); //Starts generating the route in scene based on data from the Route class
        }
    }

    /// <summary>
    /// Creates the route in-scene
    /// </summary>
    /// <returns></returns>
    private IEnumerator RouteToMap()
    {
        while (!route.DataLoaded) //If the data the route is based on hasn't been loaded yet, return null
        {
            yield return null;
        }

        print("Creating route on map");

        //Runs through all the route coordinates to create points and route objects in between these points
        //i starts at 1 meaning, that the "current" index for route is (i - 1) and i is the distination,
        //because it's zero-indexed
        for (int i = 1; i < route.RouteInMercCoords.Count; i++)
        {
            GameObject routePlane = GameObject.CreatePrimitive(PrimitiveType.Plane); //Create plane game object

            float distance = Vector2.Distance(route.RouteInMercCoords[i - 1], route.RouteInMercCoords[i]); //Calculates distance between current point and the next

            Vector2 diff = new Vector2(route.RouteInMercCoords[i].x - route.RouteInMercCoords[i - 1].x,
                route.RouteInMercCoords[i].y - route.RouteInMercCoords[i - 1].y); //Gets the vector in between the current point vector and the distination point vector

            Vector3 middlePoint = new Vector3((diff.x / 2) + route.RouteInMercCoords[i - 1].x,
                0.5f, (diff.y / 2) + route.RouteInMercCoords[i - 1].y);//Gets the point exactly between the current point and the distination point

            //Creates route object
            routePlane.transform.SetParent(mapParent);
            routePlane.transform.position = middlePoint;
            routePlane.name = "Route between: " + i + " - " + (i + 1);
            routePlane.transform.localScale = new Vector3(0.25f, 0, distance / 10);
            routePlane.GetComponent<Renderer>().material = Resources.Load<Material>("DebugRoute");
            routePlane.transform.LookAt(new Vector3(route.RouteInMercCoords[i].x, 0.5f, route.RouteInMercCoords[i].y));
            routePlanes.Add(routePlane);

            //Create point gameobject
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.name = "Route point " + i;
            point.transform.SetParent(mapParent);
            point.transform.position = new Vector3(route.RouteInMercCoords[i - 1].x, 10, route.RouteInMercCoords[i - 1].y);
            point.transform.localScale = new Vector3(7.5f, 7.5f, 7.5f);
            point.GetComponent<Collider>().isTrigger = true;
            points.Add(point);

            //If it's the last run-through of the for-loop, add another sphere.
            //This is done because we start with i = 1, but the condition is still set as i < list.Count.
            //We can't change it to i <= list.Count, as we use both i-1 and i+1 within this loop, and that would create an exception
            //This means that we have to add the following code to finish the route properly. 
            //Else we have a routePlane connecting to only 1 point, making the route impossible to complete
            if (i == route.RouteInMercCoords.Count - 1)
            {
                //Create point gameobject
                GameObject point2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point2.name = "Route point " + (i + 1);
                point2.transform.SetParent(mapParent);
                point2.transform.position = new Vector3(route.RouteInMercCoords[i].x, 10, route.RouteInMercCoords[i].y);
                point2.transform.localScale = new Vector3(7.5f, 7.5f, 7.5f);
                point2.GetComponent<Collider>().isTrigger = true;
                points.Add(point2);
            }
        }
        routePlanes[0].GetComponent<Renderer>().material = Resources.Load<Material>("DebugRouteHighlight"); //Shows the first route as highlighted

        print("Route Done: Route Distance: " + route.Distance + " meters - Route Time: " + route.EstimatedTime);
        CreateGameLocations();

        //DEBUG CODE TO SHOW VIA POINTS
        //Find the tile the player stands in
        Vector2 vector = GM.LatLonToMeters(route.RouteLatLongs[0].x, route.RouteLatLongs[0].y);
        Vector2 tile = GM.MetersToTile(vector, 16);

        Vector2 centerInMercator = GM.TileBounds(tile, 16).center; //Finds the center of the tile
        int index = 1;
        foreach (Vector2 latLong in route.ViaLatLongs)
        {
            GameObject viaPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
            viaPoint.name = "Via " + index;
            viaPoint.transform.SetParent(mapParent);

            vector = GM.LatLonToMeters(latLong.x, latLong.y);

            viaPoint.transform.position = (vector - centerInMercator).ToVector3xz();
            viaPoint.transform.localScale = new Vector3(7.5f, 50f, 7.5f);

            viaPoint.GetComponent<Renderer>().material = Resources.Load<Material>("DebugViaPoints");
            Destroy(viaPoint.GetComponent<BoxCollider>());

            debugViaPoints.Add(viaPoint);
            index++;
        }
        //END DEBUG EKSTRA

        routeInUse = true; //Route is now created and in use

        PointManager.Instance.StartRouteTimer();
    }

    /// <summary>
    /// Handles the completion of a route
    /// </summary>
    public void EndRoute()
    {
        //Clears the routePlanes from the game
        foreach (GameObject route in routePlanes)
        {
            Destroy(route);
        }
        //Clears points from the game
        foreach (GameObject point in points)
        {
            Destroy(point);
        }
        //Clears game locations from the game
        foreach (GameLocation loc in gamelocations)
        {
            Destroy(loc.gameObject);
        }

        //Clears the lists
        points.Clear();
        routePlanes.Clear();
        gamelocations.Clear();

        UIController.Instance.pnlEndRoute.gameObject.SetActive(true);
    }

    /// <summary>
    /// Updates route by highighting the next route
    /// </summary>
    public void UpdateRouteForUser()
    {
        if (UIController.Instance.HitLocation == null)
        {
            if (routePlanes.Count > 0 && points.Count > 0) //Makes sure outofindex exceptions wont occur
            {
                Destroy(points[0].gameObject);
                points.RemoveAt(0);
                Destroy(routePlanes[0].gameObject);
                routePlanes.RemoveAt(0);

                if (routePlanes.Count > 0) //Makes sure outofindex exceptions wont occur
                    routePlanes[0].GetComponent<Renderer>().material = Resources.Load<Material>("DebugRouteHighlight"); //Makes the next routePlane highlighted
            }
        }
    }

    /// <summary>
    /// Creates Game Locations based in the distance of the route
    /// </summary>
    private void CreateGameLocations()
    {
        int numberOfLocations = Mathf.FloorToInt(route.Distance / 1000); //Calculates the numbers of locations needed (1 per whole kilometer)
        print("Number of game locations: " + numberOfLocations);
        int indexIncrements = route.RouteInMercCoords.Count / (numberOfLocations + 1); //Calculates the increment needed for the location to be spread out fairly evenly

        //Creates all game locations
        for (int i = 1; i <= numberOfLocations; i++)
        {
            GameLocation gameLocation = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<GameLocation>().Initialize();
            gameLocation.gameObject.name = "Game Location " + i;
            gameLocation.gameObject.transform.SetParent(mapParent);
            gameLocation.gameObject.transform.position = points[(i - 1) + (indexIncrements * i)].transform.position; //It's position is based on a point's position
            gameLocation.gameObject.transform.localScale = new Vector3(14, 14, 14);
            gameLocation.gameObject.GetComponent<Renderer>().material.color = Color.blue;
            gameLocation.gameObject.GetComponent<Collider>().isTrigger = true;
            gamelocations.Add(gameLocation);


            //Adds the routing point to the GameLocation
            gameLocation.ViaPoint = points[(i - 1) + (indexIncrements * i)];
        }
    }

    /// <summary>
    /// Starts the random routing process
    /// </summary>
    /// <param name="_length">The desired length of the route</param>
    /// <param name="_startPos">The players position in lat/long</param>
    /// <returns>Via points for route</returns>
    public List<Vector2> RandomRouting(RouteLength _length, Vector2 _startPos)
    {
        //return FourPoints(_length, _settings);
        //return StarPoints(_length, _startPos);
        return PacmanPoints(_length, _startPos);
    }

    /// <summary>
    /// Generates a random route based on four via points in a square formation
    /// </summary>
    /// <param name="_length">The desired length of the route</param>
    /// <param name="_startPos">The players position in lat/long</param>
    /// <returns>Via points for route</returns>
    private List<Vector2> FourPoints(RouteLength _length, Vector2 _startPos)
    {
        List<Vector2> fourPointVia = new List<Vector2>(); //A temp list for the via points

        float rnd = (float)_length * 3 * 0.0015f; //Distance between via points

        Vector2 dir = Vector2.zero; //Standard direction

        if (directions.Count <= 0) // If tried in all directions
        {
            return null; // Can't be done
        }

        int randomIndex = Random.Range(0, directions.Count);

        dir = directions[randomIndex];
        directions.RemoveAt(randomIndex); // So that it won't test the same direction twice.

        Vector2 otherDir = new Vector2(dir.y, dir.x); //Direction perpendicular to start direction

        //INFO:
        //The x and y coordinates of our points are latitude and longtitude
        //Which means that it's inverse of a normal coordinate system
        //Where y (west - east) is length and x (north - south) is height

        //LAT/LONG INFO
        //Latitude goes from -90 to 90 which gives 180
        //Longtitude goes from -180 to 180 which gives 360
        //Therefore we need to either double the length of latitude 
        //Or half the longtitude when converting to coordinates in our app
        //And in calculations below we decided to take half of the longtitude

        //THE MIDDLE
        Vector2 middlePoint = new Vector2(_startPos.x + ((rnd / 4) * dir.y), _startPos.y + ((rnd / 2) * dir.x)); //The imaginary point between all via points and start position

        //LEFT
        Vector2 viaPoint = new Vector2(middlePoint.x + ((rnd / 4) * otherDir.y), middlePoint.y + ((rnd / 2) * otherDir.x)); //Via point on the left side relative to start position and direction
        fourPointVia.Add(viaPoint);

        //UP
        viaPoint = new Vector2(middlePoint.x + ((rnd / 4) * dir.y), middlePoint.y + ((rnd / 2) * dir.x)); //Via point straight ahead of start position
        fourPointVia.Add(viaPoint);

        //RIGHT
        otherDir *= -1; //We need to go to right side now
        viaPoint = new Vector2(middlePoint.x + ((rnd / 4) * otherDir.y), middlePoint.y + ((rnd / 2) * otherDir.x)); //Via point on the right side relative to start position and direction

        fourPointVia.Add(viaPoint);

        return fourPointVia; //Return the via points
    }

    /// <summary>
    /// Generates random route based on eight via points in a star formation
    /// </summary>
    /// <param name="_length">The desired length of the route</param>
    /// <param name="_startPos">The players position in lat/long</param>
    /// <returns>Via points for route</returns>
    private List<Vector2> StarPoints(RouteLength _length, Vector2 _startPos)
    {
        List<Vector2> starPointVia = new List<Vector2>(); //Temp list for via points

        float bigRnd = (float)_length * 3f * 0.001f; //The large length
        float smallRnd = (float)_length * 1f * 0.0012f; //The small length

        Vector2 dir = Vector2.zero; //Standard direction

        if (directions.Count <= 0) // If tried in all directions
        {
            return null; // Can't be done
        }

        int randomIndex = Random.Range(0, directions.Count);

        dir = directions[randomIndex];
        directions.RemoveAt(randomIndex); // So that it won't test the same direction twice.

        Vector2 otherDir = new Vector2(dir.y, dir.x); //Direction perpendicular to start direction

        //INFO:
        //The x and y coordinates of our points are latitude and longtitude
        //Which means that it's inverse of a normal coordinate system
        //Where y (west - east) is length and x (north - south) is height

        //LAT/LONG INFO
        //Latitude goes from -90 to 90 which gives 180
        //Longtitude goes from -180 to 180 which gives 360
        //Therefore we need to either double the length of latitude 
        //Or half the longtitude when converting to coordinates in our app
        //And in calculations below we decided to take half of the longtitude

        //MiddlePoint
        Vector2 middlePoint = new Vector2(_startPos.x + ((bigRnd / 4) * dir.y), _startPos.y + ((bigRnd / 2) * dir.x)); //The imaginary point between all via points and start position

        //Left
        Vector2 left = new Vector2(middlePoint.x + ((bigRnd / 4) * otherDir.y), middlePoint.y + ((bigRnd / 2) * otherDir.x)); //Via point on the left side relative to start position and direction

        //Top
        Vector2 top = new Vector2(middlePoint.x + ((bigRnd / 4) * dir.y), middlePoint.y + ((bigRnd / 2) * dir.x)); //Via point straight ahead of start position

        //Right
        otherDir *= -1; //Reverse the other direction
        Vector2 right = new Vector2(middlePoint.x + ((bigRnd / 4) * otherDir.y), middlePoint.y + ((bigRnd / 2) * otherDir.x)); //Via point on the right side relative to start position and direction

        //BotLeft
        Vector2 botLeftDir = (new Vector2(_startPos.x - middlePoint.x, _startPos.y - middlePoint.y) + new Vector2(left.x - middlePoint.x, left.y - middlePoint.y)); //Vector between start position and left via point
        botLeftDir.Normalize();
        Vector2 botLeft = CalcDiagonalPlacement(middlePoint, botLeftDir, smallRnd); //The via point between the start position and left via point

        //TopLeft
        Vector2 topLeftDir = (new Vector2(left.x - middlePoint.x, left.y - middlePoint.y) + new Vector2(top.x - middlePoint.x, top.y - middlePoint.y)); //Vector between left via point and top via point
        topLeftDir.Normalize();
        Vector2 topLeft = CalcDiagonalPlacement(middlePoint, topLeftDir, smallRnd); //The via point between the left via point and top via point

        //TopRight
        Vector2 topRightDir = (new Vector2(top.x - middlePoint.x, top.y - middlePoint.y) + new Vector2(right.x - middlePoint.x, right.y - middlePoint.y)); //Vector between top via point and right via point
        topRightDir.Normalize();
        Vector2 topRight = CalcDiagonalPlacement(middlePoint, topRightDir, smallRnd); //The via point between the top via point and the right via point

        //BotRight
        Vector2 botRightDir = (new Vector2(right.x - middlePoint.x, right.y - middlePoint.y) + new Vector2(_startPos.x - middlePoint.x, _startPos.y - middlePoint.y)); //The vector between the right via point and the start position
        botRightDir.Normalize();
        Vector2 botRight = CalcDiagonalPlacement(middlePoint, botRightDir, smallRnd); //The via point between the right via point and the start position

        //Adds points to list
        starPointVia.Add(botLeft);
        starPointVia.Add(left);
        starPointVia.Add(topLeft);
        starPointVia.Add(top);
        starPointVia.Add(topRight);
        starPointVia.Add(right);
        starPointVia.Add(botRight);

        return starPointVia; //Return list
    }

    /// <summary>
    /// Generates random route based on five via points in a pacman like formation.
    /// </summary>
    /// <param name="_length">The desired length of the route</param>
    /// <param name="_startPos">The players position in lat/long</param>
    /// <returns>Via points for route</returns>
    private List<Vector2> PacmanPoints(RouteLength _length, Vector2 _startPos)
    {
        List<Vector2> pacmanPointVia = new List<Vector2>();

        float rnd = (float)_length * 3f * 0.0016f; //Distance between via points

        Vector2 dir = Vector2.zero; //Standard direction

        if (directions.Count <= 0) // If tried in all directions
        {
            return null; // Can't be done
        }

        int randomIndex = Random.Range(0, directions.Count);

        dir = directions[randomIndex];
        directions.RemoveAt(randomIndex); // So that it won't test the same direction twice.

        Vector2 otherDir = new Vector2(dir.y, dir.x); //Direction perpendicular to start direction

        //INFO:
        //The x and y coordinates of our points are latitude and longtitude
        //Which means that it's inverse of a normal coordinate system
        //Where y (west - east) is length and x (north - south) is height

        //LAT/LONG INFO
        //Latitude goes from -90 to 90 which gives 180
        //Longtitude goes from -180 to 180 which gives 360
        //Therefore we need to either double the length of latitude 
        //Or half the longtitude when converting to coordinates in our app
        //And in calculations below we decided to take half of the longtitude

        //LEFT
        Vector2 viaPoint = new Vector2(_startPos.x + ((rnd / 4) * otherDir.y), _startPos.y + ((rnd / 2) * otherDir.x)); //Via point on the left side relative to start position and direction
        pacmanPointVia.Add(viaPoint);

        //UP
        viaPoint = new Vector2(_startPos.x + ((rnd / 4) * dir.y), _startPos.y + ((rnd / 2) * dir.x)); //Via point straight ahead of start position
        pacmanPointVia.Add(viaPoint);

        //RIGHT
        otherDir *= -1; //We need to go to right side now
        viaPoint = new Vector2(_startPos.x + ((rnd / 4) * otherDir.y), _startPos.y + ((rnd / 2) * otherDir.x)); //Via point on the right side relative to start position and direction
        pacmanPointVia.Add(viaPoint);

        //Down
        dir *= -1;
        viaPoint = new Vector2(_startPos.x + ((rnd / 4) * dir.y), _startPos.y + ((rnd / 2) * dir.x)); //Via point behind the start position
        pacmanPointVia.Add(viaPoint);

        return pacmanPointVia;
    }

    /// <summary>
    /// Recalculates the via points for the route
    /// </summary>
    /// <param name="_startPosition">The position of the player in the world</param>
    /// <param name="_routeLength">The desired length of the route</param>
    public void RecalculateViaPoints(Vector2 _startPosition, RouteLength _routeLength)
    {
        print("Tried again");

        List<Vector2> newVias = RandomRouting(_routeLength, _startPosition); //Generates new via points

        if (newVias != null) //If list isn't null let's try creating the route again
            route.Retry(_startPosition, newVias);
        else //Else no route was avaiable
            CouldNotFindAnyRoute();
    }

    /// <summary>
    /// Handling what happens if no route could be created
    /// </summary>
    private void CouldNotFindAnyRoute()
    {
        print("Could not find any route");

        ErrorPanel.Instance.ShowError("Couldn't find a route",
            "It seems no route was available, for your prefered route length.\nMaybe try another route length, or go to another location and try again", ErrorType.COULD_NOT_FIND_ROUTE);
    }

    /// <summary>
    /// Finds the direction perpendicular to given direction
    /// </summary>
    /// <param name="direction">The direction</param>
    /// <returns>The perpendicular direction</returns>
    private Vector2 FindTheOtherDirection(Vector2 direction)
    {
        Vector2 otherDir;

        if (direction.x == 0 || direction.y == 0) //If direction is horizontal or vertical
            otherDir = new Vector2(direction.y, direction.x); //Swich the values
        else //If the direction is diagonal
            otherDir = new Vector2(direction.x * -1, direction.y); //Time the x value by -1

        return otherDir; //Return perpendicular direction
    }

    /// <summary>
    /// Calculates the placement of diagonal via points
    /// </summary>
    /// <param name="_middelPoint">The imaginary middle point</param>
    /// <param name="_direction">The direction from middle point</param>
    /// <param name="distance">The distance from middle point</param>
    /// <returns>Diagonal via point</returns>
    private Vector2 CalcDiagonalPlacement(Vector2 _middelPoint, Vector2 _direction, float distance)
    {
        //if (_direction.x == 0 || _direction.y == 0)
        //    return _middelPoint + new Vector2(_direction.x * 0.5f, _direction.y) * distance;
        //else
        return _middelPoint + new Vector2(_direction.x * 0.5f, _direction.y) * distance;
    }

}
