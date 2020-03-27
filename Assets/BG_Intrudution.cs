using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Intrudution : MonoBehaviour
{
    public string[] info;

    public AudioClip[] BGM;

    float volume;
    Action uAction;

    AudioSource audio;

    public void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    public void SetNewBGM()
    {
        volume = 0;
        audio.volume = 0;
        int sel = UnityEngine.Random.Range(0, BGM.Length);
        audio.clip = BGM[sel];
        audio.Play();
        uAction = VolumeUp;
    }

    void VolumeUp()
    {
        volume += Time.deltaTime;
        audio.volume = volume;

        if (volume >= 1)
        {
            uAction = null;
        }
    }

    private void Update()
    {
        if (uAction != null)
        {
            uAction();
        }
    }
}
