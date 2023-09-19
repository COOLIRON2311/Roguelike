using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    public float lowPitchRange = .95f; // slight variations
    public float highPitchRange = 1.05f; // at how sounds are played

    void Awake()
    {
        if (instance == null) // singleton code
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Play single audio clip
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randIdx = Random.Range(0, clips.Length); // choose random clip to play
        float randPitch = Random.Range(lowPitchRange, highPitchRange); // and random pitch

        efxSource.pitch = randPitch;
        efxSource.clip = clips[randIdx];
        efxSource.Play();
    }
}
