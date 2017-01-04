using UnityEngine;
using System.Collections;

/// <summary>
/// Initializes the GPS
/// </summary>
public class InitGPS : MonoBehaviour {

	/// <summary>
    /// Unity Start Method
    /// </summary>
	void Start () {
#if !UNITY_EDITOR
        StartCoroutine(Init());
#endif
    }

    /// <summary>
    /// Coroutines used on devices with GPS to initialize the map
    /// </summary>
    /// <returns></returns>
    private IEnumerator Init()
    {
        if (!Input.location.isEnabledByUser) // First, check if user has location service enabled
        {
            ErrorPanel.Instance.ShowError("GPS Inactive", 
                "Your GPS/Location Service is inactive\nPlease activite your GPS/Locations Service and try again", 
                ErrorType.GPS_INACTIVE);
            yield break;
        }

        Input.location.Start(5, 5); // Start service before querying location

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            ErrorPanel.Instance.ShowError("GPS timed out", 
                "GPS timed out when trying to initialize\nClick /OK/ to try again.", 
                ErrorType.GPS_TIMED_OUT);
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            ErrorPanel.Instance.ShowError("GPS Initialization failed", 
                "Failed to start GPS service\nClick /OK/ to try again.", 
                ErrorType.GPS_INITIALIZATION_FAILED);
            yield break;
        }
        else
        {
            print("Location Service connection established.");
        }
    }

    /// <summary>
    /// Method to reinit the GPS
    /// </summary>
    public void ReinitLocationService()
    {
#if !UNITY_EDITOR
        Input.location.Stop();
        StartCoroutine(Init());
#endif
    }
}
