using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BG_Control : MonoBehaviour
{
    public List<VideoClip> vcs;

    public VideoPlayer introBG;
    public VideoPlayer inGameBG;


    private void OnEnable()
    {
        int r1 = Random.Range(0, vcs.Count);
        int r2 = Random.Range(0, vcs.Count);

        while (r1 == r2) {
            r2 = Random.Range(0, vcs.Count);
        }

        introBG.clip = vcs[r1];
        inGameBG.clip = vcs[r2];
    }
}
