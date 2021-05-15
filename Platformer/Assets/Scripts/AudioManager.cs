using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        // singleton
        if (instance == null)
        {
            instance = this;
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.loop = s.shouldLoop;
            }
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) { return; }
        s.source.Play();
    }

    private void Start()
    {
        Play("Theme");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
