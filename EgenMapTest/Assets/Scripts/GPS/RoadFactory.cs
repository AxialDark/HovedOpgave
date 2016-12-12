using UnityEngine;
using System.Collections.Generic;
using Assets.Helpers;

/// <summary>
/// Used for initializing construction for a road
/// Based on data from mapzen
/// </summary>
public class RoadFactory : MonoBehaviour
{
    /// <summary>
    /// A list of all the roads
    /// </summary>
    public List<RoadPolygon> MyRoads { get; private set; }

    /// <summary>
    /// Unity method is run at the beginning
    /// </summary>
    private void Start()
    {
        MyRoads = new List<RoadPolygon>();
    }

    /// <summary>
    /// Creates a road on the map
    /// </summary>
    /// <param name="_tileMercPos">The XY coordinates of the tile</param>
    /// <param name="_geo">The geo data from Mapzen</param>
    /// <param name="_index">The road index in geo data</param>
    /// <param name="_tileTransform">The transfrom of the tile the road is on</param>
    public void CreateRoad(Vector2 _tileMercPos, JSONObject _geo, int _index, Transform _tileTransform)
    {
        if (_geo["geometry"]["type"].str == "LineString") //If road is a LineString
        {
            List<Vector3> roadEnds = new List<Vector3>();

            for (int i = 0; i < _geo["geometry"]["coordinates"].list.Count; i++) //Run through all coordinates
            {
                JSONObject coordinate = _geo["geometry"]["coordinates"][i]; //Saves the JSONObject at the i coordinate
                Vector2 dotMerc = GM.LatLonToMeters(coordinate[1].f, coordinate[0].f); //Converts latitude and longtitude to XY mercator meters
                Vector2 localMercPos = new Vector2(dotMerc.x - _tileMercPos.x, dotMerc.y - _tileMercPos.y); //Finds the right position for road
                roadEnds.Add(localMercPos.ToVector3xz()); //Adds the road to road list
            }

            CreateRoadSegment(_tileTransform, _index, _geo, roadEnds);
        }
        else if (_geo["geometry"]["type"].str == "MultiLineString")
        {
            for (int i = 0; i < _geo["geometry"]["coordinates"].list.Count; i++) //Run through all coordinates
            {
                List<Vector3> roadEnds = new List<Vector3>(); 
                JSONObject coordinate = _geo["geometry"]["coordinates"][i]; //Saves the JSONObject at the i coordinate

                for (int j = 0; j < coordinate.list.Count; j++) //Runs trough all segments of the road coordinate
                {
                    JSONObject segment = coordinate[j];  //Find the current segment
                    Vector2 dotMerc = GM.LatLonToMeters(segment[1].f, segment[0].f); //Converts latitude and longtitude to XY mercator meters
                    Vector2 localMercPos = new Vector2(dotMerc.x - _tileMercPos.x, dotMerc.y - _tileMercPos.y); //Finds the right position for road segment
                    roadEnds.Add(localMercPos.ToVector3xz()); //Adds the segment to road list
                }

                CreateRoadSegment(_tileTransform, _index, _geo, roadEnds);
            }
        }
    }

    /// <summary>
    /// Creates the roads for the tile
    /// </summary>
    /// <param name="_tileTransform">The transform of the tile</param>
    /// <param name="_index">The index of the road</param>
    /// <param name="_geo">The geographical data from mapzen</param>
    /// <param name="_roadEnds">All the points where the roads end</param>
    private void CreateRoadSegment(Transform _tileTransform, int _index, JSONObject _geo, List<Vector3> _roadEnds)
    {
        RoadPolygon m = new GameObject("Road" + _index).AddComponent<RoadPolygon>(); //Creates a part of the road

        MyRoads.Add(m);

        m.transform.SetParent(_tileTransform, true); //Set the tile as the road segments parent
        try
        {
            m.Initialize(_geo["properties"]["id"].str, _tileTransform.position, _roadEnds, _geo["properties"]["kind"].str); //Create and place the road
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }
    }
}