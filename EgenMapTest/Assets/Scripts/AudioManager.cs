using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] soundSprites;
    [SerializeField]
    private bool isMuted = false;
    [SerializeField]
    private Image buttonSprite;

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
    void Awake()
    {
        buttonSprite.sprite = soundSprites[0];
        outOfFocus = false;
    }

    // Update is called once per frame
    void Update()
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