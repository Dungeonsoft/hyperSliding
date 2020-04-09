using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Introdution : MonoBehaviour
{
    public string[] info;

    //public AudioClip[] bmIntro;
    //public AudioClip[] bmIngame;

    public string bgmIntroPath;
    public string bgmIngamePath;

    public string[] bmIntroName;
    public string[] bmIngameName;

    float volume;
    Action uAction;

    AudioSource audio;
    AudioClip aClipIngame;

    public void Awake()
    {
        audio = GetComponent<AudioSource>();
    }


    /// <summary>
    /// 인트로 음악 선택 플레이, 인게임 음악 미러 선택.
    /// </summary>
    public void SetNewBgmIntro()
    {
        volume = 0;
        audio.volume = 0;

        // 인트로 배경음악 선택하여 플레이.
        int sel = UnityEngine.Random.Range(0, bmIntroName.Length);
        audio.clip = Resources.Load(bgmIntroPath+"/"+ bmIntroName[sel]) as AudioClip;
        audio.Play();
        uAction = VolumeUp;


        // 인게임 배경음악 미리 선택해 놓음.
        sel = UnityEngine.Random.Range(0, bmIngameName.Length);
        aClipIngame = Resources.Load(bgmIngamePath + "/" + bmIngameName[sel]) as AudioClip;
    }


    public AudioClip countSound;

    IEnumerator IngameBGM()
    {
        audio.clip = countSound;

        yield return new WaitForSeconds(countSound.length);
        audio.clip = aClipIngame;
        audio.Play();
    }

    /// <summary>
    /// 인게임 배경음악 미리 선택된 것 플레이.
    /// </summary>
    public void SetNewBgmIngame()
    {
        StartCoroutine(IngameBGM());
        audio.Play();
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
        uAction?.Invoke();
    }
}
