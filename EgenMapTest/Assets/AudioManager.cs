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
    private AudioSource backgroundPlayer;

    [SerializeField]
    private AudioClip[] backgroundAudioClips;

    [SerializeField]
    private AudioSource effectPlayer;

    [SerializeField]
    private AudioClip[] effectClips;

    private string currentEffect;

    private bool outOfFocus;

	// Use this for initialization
	void Awake () {

        outOfFocus = false;
        //backgroundPlayer.clip = GetRandomBackgroundAudio();
        //backgroundPlayer.Play();
        //print(backgroundPlayer.clip.name);
	}
	
	// Update is called once per frame
	void Update () 
    {
        //if (!backgroundPlayer.isPlaying && !outOfFocus)
        //{
        //    backgroundPlayer.Stop();
        //    backgroundPlayer.clip = GetRandomBackgroundAudio();
        //    backgroundPlayer.Play();
        //    print(backgroundPlayer.clip.name);
        //}
	}

    /// <summary>
    /// Gets a random Audioclip from array
    /// </summary>
    /// <returns>Random AudioClip</returns>
    private AudioClip GetRandomBackgroundAudio()
    {
        if (backgroundPlayer.clip != null)
        {
            AudioClip newClip = backgroundAudioClips[Random.Range(0, backgroundAudioClips.Length)];

            while (newClip == backgroundPlayer.clip && backgroundAudioClips.Length > 1)
            {
                newClip = backgroundAudioClips[Random.Range(0, backgroundAudioClips.Length)];
            }
            return newClip;
        }
        else
        {
            return backgroundAudioClips[Random.Range(0, backgroundAudioClips.Length)];
        }
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
            //case "Attack":
            //    effectPlayer.clip = effectClips[1];
            //    currentEffect = "Attack";
            //    effectPlayer.PlayDelayed(delay);
            //    break;

            //case "Die":
                
            //    break;

            //case "Move":
            //    effectPlayer.clip = effectClips[0];
            //    currentEffect = "Move";
            //    effectPlayer.loop = true;
            //    effectPlayer.PlayDelayed(delay);
            //    break;

            case "BallBounce":
                effectPlayer.clip = effectClips[0];
                currentEffect = "BallBounce";
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Stops the Move Soundeffect
    /// </summary>
    public void StopMoveEffect()
    {
        if (currentEffect == "Move")
        {
            effectPlayer.loop = false;
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