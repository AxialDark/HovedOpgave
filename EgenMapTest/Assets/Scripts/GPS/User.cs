using UnityEngine;
using Assets.Helpers;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Singleton class of the user in game
/// </summary>
public class User : MonoBehaviour
{

    private static User instance;
    private Vector2 centerInMerc;
    private Vector2 lastLatLong;
    private Vector3 newPosition;

    /// <summary>
    /// Singleton in Unity
    /// </summary>
    public static User Instance
    {
        get
        {
            if (instance == null)
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

        Invoke("UpdatePosition", 2); //Updates the users position every indicated interval in seconds
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
    /// Updates the users position on the GPS using GPS data.
    /// </summary>
    private void UpdatePosition()
    {
        CancelInvoke();

        if (Input.location.status == LocationServiceStatus.Running)
        {
            LocationInfo info = Input.location.lastData; //Collects GPS data from device
            if (new Vector2(info.latitude, info.longitude) == LastLatLong) return; //return if the same as last update

            Vector2 currentLatLong = new Vector2(info.latitude, info.longitude); //Create vector using the new data

            Vector2 vector = GM.LatLonToMeters(currentLatLong); //Convert LatLong to mercator coordinates
            newPosition = (vector - centerInMerc).ToVector3xz(); //Creates new position relative to the center of the tile
            StartCoroutine(SmoothMovement(2));
            lastLatLong = currentLatLong; //Updates last LatLong
        }
        else
        {
            ErrorPanel.Instance.ShowError("GPS connection lost.", "You no longer have a GPS connection", ErrorType.GPS_STATE_NOT_RUNNING);
        }

    }

    /// <summary>
    /// Makes the movement between last position and new position smooth
    /// <param name="value">The time it should take to move to the new position</param>
    /// </summary>
    private IEnumerator SmoothMovement(float value)
    {
        float rate = 1.0f / value;
        float t = 0.0f;
        while (t < 1.0)
        {
            t += Time.deltaTime * rate;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, newPosition, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }
    }

    /// <summary>
    /// Unity Method.
    /// Detects when colliders enters each other.
    /// </summary>
    /// <param name="_other"></param>
    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.GetComponent<GameLocation>())
        {
            if (Application.platform == RuntimePlatform.Android) Handheld.Vibrate();
        }
    }

    /// <summary>
    /// Unity method.
    /// Continuously detect when colliders stay within each other.
    /// </summary>
    /// <param name="other">The detected trigger collider</param>
    private void OnTriggerStay(Collider _other)
    {
        if (RouteManager.Instance.Points.Count - 1 > 0 && _other.gameObject == RouteManager.Instance.Points[1]) //If list contains 2 or more points, update it when colliding
        {
            RouteManager.Instance.UpdateRouteForUser();
            print("Collision with RoutePoint");
        }

        if (_other.gameObject.GetComponent<GameLocation>())
        {
            //UIController.Instance.btnStartGame.gameObject.SetActive(true);
            UIController.Instance.HitGameLocation(_other.gameObject.GetComponent<GameLocation>());
        }
    }

    /// <summary>
    /// Unity method.
    /// Detects when colliders exit each other.
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
