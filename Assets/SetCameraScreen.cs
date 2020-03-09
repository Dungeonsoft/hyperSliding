using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraScreen : MonoBehaviour
{
    public Transform PuzzleTf;
    // Start is called before the first frame update
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(720, 1280, true);

        //Debug.Log("Screen Width : "+Screen.width);
        //Debug.Log("Screen Height : " + Screen.height);

        float screenRatio = 1280f / 720f;
        float newScreenRatio = Screen.height*1f / Screen.width;

        Debug.Log("screenRatio : " + screenRatio);
        Debug.Log("newScreenRatio : " + newScreenRatio);


        float changeScaleZ = screenRatio / newScreenRatio;
        float changeScaleX =  newScreenRatio/ screenRatio;

        Debug.Log("Change Sclale : "+ changeScaleZ);

        PuzzleTf.localScale = new Vector3(1f, 1f, changeScaleZ);
    }

}
