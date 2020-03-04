using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerFixLoopScript : MonoBehaviour
{
    public VideoPlayer vp;
    public Texture firstFarame;
    // Start is called before the first frame update
    void Start()
    {
        //vp.targetTexture.DiscardContents();
        //vp.targetTexture.Release();

        Graphics.Blit(firstFarame, vp.targetTexture);

        vp.loopPointReached += EndReached;
    }


    void EndReached(VideoPlayer v)
    {
        Debug.Log("Check01");
        Graphics.Blit(firstFarame, vp.targetTexture);

        //v.targetTexture.Release();
    }
}
