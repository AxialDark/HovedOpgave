using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Helpers;
using System.Linq;

public class BuildingFactory : MonoBehaviour {

    [SerializeField]
    private Building.Settings settings;
    private Dictionary<Vector3, Building> Buildings { get; set; } //Apparently this is not good to use (Building center as key)

    private void Start()
    {
        Buildings = new Dictionary<Vector3, Building>();
    }

    public void CreateBuilding(Vector2 _tileMercPos, JSONObject _geo, Transform _parent = null)
    {
        _parent = _parent ?? transform; //If _parent is null, transform becomes parent
        List<Vector3> buildingCorners = new List<Vector3>();

        JSONObject buildingJ = _geo["geometry"]["coordinates"].list[0];
        for (int i = 0; i < buildingJ.list.Count - 1; i++)
        {
            JSONObject coordinate = buildingJ.list[i];
            Vector2 dotMerc = GM.LatLonToMeters(coordinate[1].f, coordinate[0].f);
            Vector2 localMercPos = new Vector2(dotMerc.x - _tileMercPos.x, dotMerc.y - _tileMercPos.y);
            buildingCorners.Add(localMercPos.ToVector3xz());
        }

        Vector3 uniqPos = new Vector3(buildingJ.list[0][1].f, 0, buildingJ[0][0].f);
        if (Buildings.ContainsKey(uniqPos))
        {
            return;
        }

        try
        {
            Vector3 buildingCenter = buildingCorners.Aggregate((acc, cur) => acc + cur) / buildingCorners.Count; //Finds the average value (middle of building)

            if (!Buildings.ContainsKey(buildingCenter))
            {
                for (int i = 0; i < buildingCorners.Count; i++)
                {
                    buildingCorners[i] = buildingCorners[i] - buildingCenter;
                }

                Building building = new GameObject().AddComponent<Building>();
                string kind = "";

                if (_geo["properties"].HasField("landuse_kind"))
                {
                    kind = _geo["properties"]["landuse_kind"].str;
                }
                if (_geo["properties"].HasField("name"))
                {
                    building.name = _geo["properties"]["name"].str;
                }

                building.Initialize(buildingCorners, kind, settings);

                building.gameObject.name = "building";
                building.transform.parent = _parent;
                building.transform.localPosition = buildingCenter;
                Buildings.Add(uniqPos, building);
            }
        }
        catch(System.Exception ex)
        {
            Debug.Log(ex);
        }
    }
}