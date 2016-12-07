using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    #region Singleton
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<AudioManager>();
            }
            return instance;
        }
    }
    #endregion

    //Fields
    [SerializeField]
    private AudioSource effectPlayer;

    [SerializeField]
    private AudioClip[] effectClips;

    private string currentEffect;

    private bool outOfFocus;

	// Use this for initialization
	void Awake ()
    {
        outOfFocus = false;
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    /// <summary>
    /// Plays an effect with delay
    /// </summary>
    /// <param name="effectName">Name of the effect to play</param>
    /// <param name="delay">Delay in seconds</param>
    public void PlayEffect(string effectName, float delay = 0)
    {
        switch (effectName)
        {
            case "BallBounce":
                effectPlayer.clip = effectClips[0];
                currentEffect = "BallBounce";
                effectPlayer.Play();
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// When application comes into focus
    /// </summary>
    /// <param name="focusStatus">Is it in focus</param>
    void OnApplicationFocus(bool focusStatus)
    {
        outOfFocus = focusStatus;
    }

    /// <summary>
    /// When application comes out of fokus
    /// </summary>
    /// <param name="pauseStatus">Is application paused</param>
    void OnApplicationPause(bool pauseStatus)
    {
        outOfFocus = pauseStatus;
    }
}