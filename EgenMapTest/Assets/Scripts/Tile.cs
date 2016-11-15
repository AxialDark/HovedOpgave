using UnityEngine;
using System.Collections;
using Assets.Helpers;
using UniRx;
using System.Linq;

public class Tile : MonoBehaviour
{

    public string mapImageBaseUrl = "http://b.tile.openstreetmap.org/";

    [SerializeField]
    public Rect rect;

    private RoadFactory roadFac;
    private BuildingFactory buildFac;
    private Settings settings;

    public Tile Initialize(BuildingFactory _buildFac, RoadFactory _roadFac, Settings _settings)
    {
        roadFac = _roadFac;
        buildFac = _buildFac;
        settings = _settings;
        rect = GM.TileBounds(_settings.TileTMS, _settings.Zoom);

        return this;
    }

    public void ConstructTile(string _text)
    {
        ConstructAsync(_text);
    }

    private void ConstructAsync(string _text)
    {
        string url;

        IObservable<JSONObject> heavyMethod = Observable.Start(() => new JSONObject(_text));

        heavyMethod.ObserveOnMainThread().Subscribe(mapData =>
        {
            if (!this) //Checks if the tile still exists and haven't destroyed yet
                return;

            Transform gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane).transform; //Creates af plane
            gameObject.name = "map"; //Names the gameplane
            gameObject.localScale = new Vector3(rect.width / 10, 1, rect.width / 10); //Resizes the plane to a tenth of it's size
            gameObject.rotation = Quaternion.AngleAxis(180, new Vector3(0, 1, 0)); //Rotates the plane on y axis 180 degress
            gameObject.parent = this.transform; //Sets the this tile as the parent
            gameObject.localPosition = Vector3.zero; //Resets the local position
            gameObject.localPosition -= new Vector3(0, 1, 0); //Moves the plane down 1 on the y axis
            Renderer rend = gameObject.GetComponent<Renderer>(); //Get's the rendere from the gameobject
            rend.material = Resources.Load<Material>("Ground"); //Set's the planes material to Ground
            rend.material.mainTexture = new Texture2D(512, 512, TextureFormat.DXT5, false); //Creates a new texture for the material
            //rend.material.color = new Color(.1f, .1f, .1f, 1f); //Gives it a new color

            if (settings.LoadImages) //If image should be loaded
            {
                rend.material.color = new Color(1f, 1f, 1f, 1f); //White color
                url = mapImageBaseUrl + settings.Zoom + "/" + settings.TileTMS.x + "/" + settings.TileTMS.y + ".png"; //Creates the image url
                ObservableWWW.GetWWW(url).Subscribe(x =>
                {
                    x.LoadImageIntoTexture((Texture2D)rend.material.mainTexture);
                });
            }

            StartCoroutine(CreateBuildings(mapData["buildings"], settings.TileCenter)); //Creates buildings
            StartCoroutine(CreateRoads(mapData["roads"], settings.TileCenter));
        });
    }

    private IEnumerator CreateBuildings(JSONObject _mapData, Vector2 _tileMercPos)
    {
        foreach (JSONObject geo in _mapData["features"].list.Where(x => x["geometry"]["type"].str == "Polygon"))
        {
            buildFac.CreateBuilding(_tileMercPos, geo, transform);
            yield return null;
        }
    }

    private IEnumerator CreateRoads(JSONObject _mapData, Vector2 _tileMercPos)
    {
        for (int index = 0; index < _mapData["features"].list.Count; index++) //Runs through all of the roads in the _mapData JSONObject
        {
            JSONObject geo = _mapData["features"].list[index]; //Finds the road at index
            roadFac.CreateRoad(_tileMercPos, geo, index, transform); //Creates the road
            yield return null; //Waits till next iteration
        }
    }

    public class Settings
    {
        public int Zoom { get; set; }
        public Vector2 TileTMS { get; set; }
        public Vector3 TileCenter { get; set; }
        public bool LoadImages { get; set; }
    }
}