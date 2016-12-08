using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Helpers;

/// <summary>
/// Enumeration that descripes the type of road
/// </summary>
public enum RoadType
{
    PATH,
    RAIL,
    MINORROAD,
    MAJORROAD,
    HIGHWAY
}

/// <summary>
/// Creates a road by combining a lot of small objects
/// Road is made based on data from mapzen
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadPolygon : MonoBehaviour
{
    /// <summary>
    /// List of renderes for all of the road parts
    /// </summary>
    public List<Renderer> MyRenderes { get; private set; }



    /// <summary>
    /// The ID for the road
    /// </summary>
    public string ID { get; set; }
    /// <summary>
    /// The type of road
    /// </summary>
    public RoadType Type { get; set; }
    private List<Vector3> verts;

    /// <summary>
    /// Creates and places the roads on the map
    /// </summary>
    /// <param name="_id">Id from mapzen data</param>
    /// <param name="_tile">The tile the road belong to</param>
    /// <param name="_verts">All the verts for contructing the road</param>
    /// <param name="_kind">What kind of road is it</param>
    public void Initialize(string _id, Vector3 _tile, List<Vector3> _verts, string _kind)
    {
        MyRenderes = new List<Renderer>();
        ID = _id;
        Type = _kind.ToRoadType();
        verts = _verts;

        for (int index = 1; index < verts.Count; index++) //Runs through all vertices for the road
        {
            GameObject roadPlane = Instantiate(Resources.Load<GameObject>(WorldMap.colorPalet + "/RoadQuad")); //Create a road

            MeshRenderer rend = roadPlane.GetComponentInChildren<MeshRenderer>();
            MyRenderes.Add(rend);
            rend.material = Resources.Load<Material>(WorldMap.colorPalet + "/Road"); //Changes material to road material



            roadPlane.transform.position = (_tile + verts[index] + _tile + verts[index - 1]) / 2; //Places the road
            roadPlane.transform.SetParent(transform, true); //Set's the parent (it looks nice in the inspector)
            Vector3 scale = roadPlane.transform.localScale; //Grab the scale of the road
            scale.z = Vector3.Distance(verts[index], verts[index - 1]) / 10; //Scale it on the z axis
            scale.x = ((float)(int)Type + 1) / 4; //Scale it on the x axis
            roadPlane.transform.localScale = scale; //Put the scale back
            roadPlane.transform.LookAt(_tile + verts[index - 1]); //Rotate the road
        }
    }
}