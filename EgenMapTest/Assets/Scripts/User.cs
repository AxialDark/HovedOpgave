using UnityEngine;
using Assets.Helpers;
using System.Collections;

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
    /// Initialize the user object
    /// </summary>
    /// <param name="_initLatLong">The latitide/longitude position</param>
    /// <param name="_centerInMerc">The tile's center position in mercator</param>
    /// <param name="_parent">The parent transform</param>
    public void Initialize(Vector2 _initLatLong, Vector2 _centerInMerc, Transform _parent)
    {
        Vector2 vector = GM.LatLonToMeters(_initLatLong);
        gameObject.transform.SetParent(_parent);
        gameObject.transform.position = (vector - _centerInMerc).ToVector3xz();
        gameObject.GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0, 255);
        gameObject.transform.localScale = new Vector3(20, 20, 20);
        Camera.main.transform.SetParent(gameObject.transform, true);
        Vector3 camPos = Vector3.zero;
        camPos.y = Camera.main.transform.localPosition.y;
        Camera.main.transform.localPosition = camPos;
        centerInMerc = _centerInMerc;
        lastLatLong = _initLatLong;
    }

    /// <summary>
    /// Unity build-in update method
    /// </summary>
    void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor) return;

        SmoothMovement();

        LocationInfo info = Input.location.lastData;
        if (new Vector2(info.latitude, info.longitude) == lastLatLong) return;

        Vector2 currentLatLong = new Vector2(info.latitude, info.longitude);

        Vector2 vector = GM.LatLonToMeters(currentLatLong);
        newPosition = (vector - centerInMerc).ToVector3xz();
        lastLatLong = currentLatLong;
    }

    /// <summary>
    /// Create user gameobject and adds User script to that object
    /// </summary>
    /// <returns>User script</returns>
    private static User CreateUserObject()
    {
        User gm = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<User>();

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
}
