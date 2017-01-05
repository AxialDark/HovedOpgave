using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.Helpers;
using System.Linq;

/// <summary>
/// Used for creating a building and places it upon the map
/// Building is made based on mapzen data
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Building : MonoBehaviour
{
    private List<Vector3> vertices;
    public string landuseKind;
    public string name;

    /// <summary>
    /// The Building's rendere
    /// </summary>
    public MeshRenderer myRendere { get; private set; }
    /// <summary>
    /// Returns the landusekind as lowercase
    /// </summary>
    public string LanduseKind { get { return landuseKind.ToLower(); } }

    /// <summary>
    /// Initializes the building
    /// </summary>
    /// <param name="_buildingCorners">Corners of the building from mapzen data</param>
    /// <param name="_kind">What kind of building is it, from mapzen data</param>
    /// <param name="_settings">The settings for the building</param>
    public void Initialize(List<Vector3> _buildingCorners, string _kind, Settings _settings)
    {
        landuseKind = string.IsNullOrEmpty(_kind) ? "default" : _kind; //If building doesn't have a kind, give it default material
        vertices = _buildingCorners;
        GetComponent<MeshFilter>().mesh = CreateMesh(_buildingCorners, _settings); //Creates a mesh for the building
        string path = ("Map Themes/" + WorldMap.colorPalet.ToString() + "/" + landuseKind).ToLower();

        myRendere = GetComponent<MeshRenderer>();
        myRendere.material = Resources.Load<Material>(path); //Gives building material depending on type of building

        //print(landuseKind);
    }

    /// <summary>
    /// Creates a mesh based on data from mapzen
    /// </summary>
    /// <param name="_verts">All the vertices for the mesh</param>
    /// <param name="_settings">The building settings</param>
    /// <returns>A unity mesh</returns>
    private static Mesh CreateMesh(List<Vector3> _verts, Settings _settings)
    {
        int height = UnityEngine.Random.Range(_settings.minHeight, _settings.maxHeight); //Gets a random building height
        Triangulator tris = new Triangulator(_verts.Select(x => x.ToVector2xz()).ToArray()); //Creates a triangulator based on building corners
        Mesh mesh = new Mesh();

        List<Vector3> vertices = _verts.Select(x => new Vector3(x.x, height, x.z)).ToList();
        List<int> indices = tris.Triangulate().ToList();

        int n = vertices.Count();


        Vector3 vertice1;
        Vector3 vertice2;
        for (int i = 1; i < _verts.Count; i++)
        {
            vertice1 = vertices[i - 1]; //Gets the first vertice
            vertice2 = vertices[i]; //Get the second vertice
            vertices.Add(vertice1); //Add vertice 1 to list
            vertices.Add(vertice2); //Add vertice 2 to list
            vertices.Add(new Vector3(vertice1.x, 0, vertice1.z));
            vertices.Add(new Vector3(vertice2.x, 0, vertice2.z));

            indices.Add(n);
            indices.Add(n + 2);
            indices.Add(n + 1);

            indices.Add(n + 1);
            indices.Add(n + 2);
            indices.Add(n + 3);

            n += 4;
        }

        vertice1 = vertices[0];
        vertice2 = vertices[_verts.Count - 1];
        vertices.Add(vertice1);
        vertices.Add(vertice2);
        vertices.Add(new Vector3(vertice1.x, 0, vertice1.z));
        vertices.Add(new Vector3(vertice2.x, 0, vertice2.z));

        indices.Add(n);
        indices.Add(n + 1);
        indices.Add(n + 2);

        indices.Add(n + 1);
        indices.Add(n + 3);
        indices.Add(n + 2);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    /// <summary>
    /// Settings for a building
    /// </summary>
    [Serializable]
    public class Settings
    {
        public int maxHeight = 0;
        public int minHeight = 0;
    }
}