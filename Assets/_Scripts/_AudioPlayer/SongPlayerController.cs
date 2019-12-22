using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongPlayerController : MonoBehaviour
{
    AudioSource songPlayer;
    // Start is called before the first frame update
    void Start()
    {
        songPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaySoundInterval(float fromSeconds, float toSeconds)
    {
        songPlayer.time = fromSeconds;
        songPlayer.Play();
        songPlayer.SetScheduledEndTime(AudioSettings.dspTime + (toSeconds - fromSeconds));
    }
}
