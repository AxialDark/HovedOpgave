using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Helpers;

public enum RoadType
{
    PATH,
    RAIL,
    MINORROAD,
    MAJORROAD,
    HIGHWAY
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
internal class RoadPolygon : MonoBehaviour
{
    public string ID { get; set; }
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
        ID = _id;
        Type = _kind.ToRoadType();
        verts = _verts;

        for (int index = 1; index < verts.Count; index++) //Runs through all vertices for the road
        {
            GameObject roadPlane = Instantiate(Resources.Load<GameObject>("RoadQuad")); //Create a road
            roadPlane.GetComponentInChildren<MeshRenderer>().material = Resources.Load<Material>("Road"); //Changes material to road material
            roadPlane.transform.position = (_tile + verts[index] + _tile + verts[index - 1]) / 2; //Places the road
            roadPlane.transform.SetParent(transform, true); //Set's the parent (it looks nice in the inspektor)
            Vector3 scale = roadPlane.transform.localScale; //Grab the scale of the road
            scale.z = Vector3.Distance(verts[index], verts[index - 1]) / 10; //Scale it on the z axis
            scale.x = ((float)(int)Type + 1) / 4; //Scale it on the x axis
            roadPlane.transform.localScale = scale; //Put the scale back
            roadPlane.transform.LookAt(_tile + verts[index - 1]); //Rotate the road
        }
    }

}