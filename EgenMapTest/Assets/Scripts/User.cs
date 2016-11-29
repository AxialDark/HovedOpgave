using UnityEngine;
using Assets.Helpers;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Singleton class of the user in game
/// </summary>
public class User : MonoBehaviour {

    private static User instance;
    private Vector2 centerInMerc;
    private Vector2 lastLatLong;
    private Vector3 newPosition;
    private float speed = 200;

    /// <summary>
    /// Singleton in Unity
    /// </summary>
    public static User Instance
    {
        get
        {
           if(instance == null)
            {
                instance = CreateUserObject();
            }
            return instance;
        }
    }

    /// <summary>
    /// Most recently recieved latitide/longitude corrdanates of the device
    /// </summary>
    public Vector2 LastLatLong
    {
        get
        {
            return lastLatLong;
        }
    }

    /// <summary>
    /// Initialize the user object
    /// </summary>
    /// <param name="_initLatLong">The latitide/longitude position</param>
    /// <param name="_centerInMerc">The tile's center position in mercator</param>
    /// <param name="_parent">The parent transform</param>
    public void Initialize(Vector2 _initLatLong, Vector2 _centerInMerc, Transform _parent)
    {
        Camera cam = GameObject.FindGameObjectWithTag("SecondCamera").GetComponent<Camera>();

        Vector2 vector = GM.LatLonToMeters(_initLatLong);
        gameObject.transform.SetParent(_parent);
        gameObject.transform.position = (vector - _centerInMerc).ToVector3xz();
        gameObject.GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0, 255);
        gameObject.transform.localScale = new Vector3(20, 20, 20);
        cam.transform.SetParent(gameObject.transform, true);
        Vector3 camPos = Vector3.zero;
        camPos.y = cam.transform.localPosition.y;
        cam.transform.localPosition = camPos;
        centerInMerc = _centerInMerc;
        lastLatLong = _initLatLong;
    }

    /// <summary>
    /// Unity build-in update method
    /// </summary>
    private void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor) return;

        SmoothMovement();

        LocationInfo info = Input.location.lastData; //Collects GPS data from device
        if (new Vector2(info.latitude, info.longitude) == LastLatLong) return; //return if the same as last update

        Vector2 currentLatLong = new Vector2(info.latitude, info.longitude); //Create vector using the new data

        Vector2 vector = GM.LatLonToMeters(currentLatLong); //Convert LatLong to mercator coordinates
        newPosition = (vector - centerInMerc).ToVector3xz(); //Creates new position relative to the center of the tile
        lastLatLong = currentLatLong; //Updates last LatLong
    }

    /// <summary>
    /// Create user gameobject and adds User script to that object
    /// </summary>
    /// <returns>User script</returns>
    private static User CreateUserObject()
    {
        User gm = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<User>();
        gm.gameObject.AddComponent<Rigidbody>();
        gm.gameObject.GetComponent<Rigidbody>().useGravity = false;

#if UNITY_EDITOR
        gm.gameObject.AddComponent<DebugMovement>();
#endif
        return gm;
    }

    /// <summary>
    /// Makes the movement between last position and new position smooth
    /// </summary>
    private void SmoothMovement()
    {
        if (newPosition == null) return;

        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, newPosition, speed * Time.deltaTime);
    }

    /// <summary>
    /// Unity method
    /// Detects collisions with triggers at run time
    /// </summary>
    /// <param name="other">The detected trigger collider</param>
    private void OnTriggerStay(Collider _other)
    {        
        if(RouteManager.Points.Count - 1 > 0 && _other.gameObject == RouteManager.Points[1]) //If list contains 2 or more points, update it when colliding
        {
            RouteManager.UpdateRouteForUser();
            print("Collision with RoutePoint");
        }

        if (_other.gameObject.GetComponent<GameLocation>())
        {
            UIController.Instance.btnStartGame.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Unity method
    /// Detects when a tho colliders exit each other
    /// </summary>
    /// <param name="_other">The detected trigger collider</param>
    private void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject.GetComponent<GameLocation>())
        {
            UIController.Instance.btnStartGame.gameObject.SetActive(false); 
        }
    }
}
