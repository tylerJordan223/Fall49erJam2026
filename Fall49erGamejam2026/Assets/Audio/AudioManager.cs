using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //information
    public float music_volume;
    public float sfx_volume;

    //audio source
    [Header("Audio Source")]
    [SerializeField] public AudioSource sfx;

    #region audios
    [SerializeField] public AudioClip rob_footstep;
    [SerializeField] public AudioClip critter_hitwall;
    [SerializeField] public AudioClip leave_terrarium;
    [SerializeField] public AudioClip spotlight;
    [SerializeField] public AudioClip startGame;
    [SerializeField] public AudioClip vacuum_start;
    [SerializeField] public AudioClip pop_guy;

    [SerializeField] public AudioClip rob_stepup;
    [SerializeField] public AudioClip rob_stepdown;
    [SerializeField] public AudioClip rob_lean;
    [SerializeField] public AudioClip rob_unlean;
    [SerializeField] public AudioClip rob_transition;


    #endregion audios

    //getting the audio instance
    public static AudioManager instance;

    private bool ignore_volume;

    private void Awake()
    {
        //Make sure there is only one instance at all times
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        //playing the normal first music
        ignore_volume = false;
    }

    public void PlaySingleSFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }

    public void PlaySFXPitched(AudioClip clip, float pitch)
    {
        sfx.pitch = pitch;
        sfx.PlayOneShot(clip);
        sfx.pitch = 1f;
    }

    private void Update()
    {
        if (!ignore_volume)
        {
            if(sfx.volume !=  sfx_volume)
            {
                sfx.volume = sfx_volume;
            }
        }
        
    }
}
