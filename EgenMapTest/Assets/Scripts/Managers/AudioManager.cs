using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// Handles the Audio in the app
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] soundSprites;
    [SerializeField]
    private bool isMuted = false;
    [SerializeField]
    private Image buttonSprite;

    [SerializeField]
    private AudioSource effectPlayer;

    [SerializeField]
    private AudioClip[] effectClips;

    private string currentEffect;

    private bool outOfFocus;

    #region Singleton
    private static AudioManager instance;

    /// <summary>
    /// Singleton instance of the AudioManager
    /// </summary>
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


    /// <summary>
    /// Unity build in Awake method
    /// Is run before Start()
    /// </summary>
    private void Awake()
    {
        buttonSprite.sprite = soundSprites[0];
        outOfFocus = false;
    }

    /// <summary>
    /// Unity build in Update method
    /// Is run once pr. frame
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// Plays an effect with delay
    /// </summary>
    /// <param name="_effectName">Name of the effect to play</param>
    /// <param name="_delay">Delay in seconds</param>
    public void PlayEffect(string _effectName, float _delay = 0)
    {
        switch (_effectName)
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
    /// Mute button event method when button is clicked
    /// </summary>
    public void OnMuteClick()
    {
        isMuted = !isMuted;
        buttonSprite.sprite = soundSprites[Convert.ToInt32(isMuted)];

        MuteSound(isMuted);
    }

    /// <summary>
    /// Mutes the sound
    /// </summary>
    /// <param name="_mute">Should it be muted</param>
    private void MuteSound(bool _mute)
    {
        effectPlayer.mute = _mute;
    }

    /// <summary>
    /// When application comes into focus
    /// </summary>
    /// <param name="_focusStatus">Is it in focus</param>
    private void OnApplicationFocus(bool _focusStatus)
    {
        outOfFocus = _focusStatus;
    }

    /// <summary>
    /// When application comes out of fokus
    /// </summary>
    /// <param name="_pauseStatus">Is application paused</param>
    private void OnApplicationPause(bool _pauseStatus)
    {
        outOfFocus = _pauseStatus;
    }
}