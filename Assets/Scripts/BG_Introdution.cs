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
        uAction = UpSound;


        // 인게임 배경음악 미리 선택해 놓음.
        sel = UnityEngine.Random.Range(0, bmIngameName.Length);
        aClipIngame = Resources.Load(bgmIngamePath + "/" + bmIngameName[sel]) as AudioClip;

        GetComponent<SoundAnalyze>().Restart();
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



    private void Update()
    {
        uAction?.Invoke();
    }

    public void GamePause()
    {
        uAction = DnSound;
    }


    public void GameUnpause()
    {
        uAction = UpSound;
    }

    public void GameComplete()
    {
        Debug.Log("게임 배경음 반으로 줄임");
        uAction = DnSoundHalf;
    }


    void UpSound()
    {
        volume += Time.deltaTime;
        audio.volume = volume;
        if(audio.volume>= 1.0f)
        {
            volume = 1.0f;
            audio.volume = volume;
            uAction = null;
        }
    }

    void DnSound()
    {
        volume -= Time.deltaTime;
        audio.volume = volume;
        if (audio.volume <= 0.0f)
        {
            volume = 0.0f;
            audio.volume = volume;
            uAction = null;
        }
    }

    void DnSoundHalf()
    {
        volume -= Time.deltaTime;
        audio.volume = volume;
        if (audio.volume <= 0.2f)
        {
            volume = 0.2f;
            audio.volume = volume;
            uAction = null;
        }
    }
}
