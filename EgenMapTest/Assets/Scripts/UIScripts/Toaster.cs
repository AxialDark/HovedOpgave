using UnityEngine;
using System.Collections;

/// <summary>
/// This class is made based on this page: https://deltabit.wordpress.com/2016/03/04/show-toast-on-unity/#more-228
/// </summary>
public class Toaster : MonoBehaviour
{
    #region Fields
    private string toastString;
    AndroidJavaObject currentActivity;
    #endregion

    /// <summary>
    /// Method for showing a given string as a toast on the android device
    /// </summary>
    /// <param name="_toastText">String to be the toast</param>
    public void ShowToast(string _toastText)
    {
        if (Application.platform == RuntimePlatform.Android) // Is the program being run on an Android device?
        {
            ShowToastOnUiThread(_toastText);
        }
    }

    /// <summary>
    /// Method for showing the toast on the ui thread.
    /// </summary>
    /// <param name="_toastString"></param>
    private void ShowToastOnUiThread(string _toastString)
    {
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        this.toastString = _toastString;

        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(ShowTheToast));
    }

    /// <summary>
    /// Method that creates and shows a given toast
    /// </summary>
    private void ShowTheToast()
    {
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", toastString);
        AndroidJavaObject toast = Toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_SHORT"));
        toast.Call("show");
    }
}
