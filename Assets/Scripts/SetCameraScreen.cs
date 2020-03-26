using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraScreen : MonoBehaviour
{
    public List<Transform> scaleShrinkObj;
    public List<Transform> scaleShrinkCvs;
    public List<Transform> scaleExpandObj;
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


        float shrinkScaleZ = screenRatio / newScreenRatio;
        float ExpandScaleZ =  newScreenRatio/ screenRatio;

        Debug.Log("Shrink Sclale : " + shrinkScaleZ);
        Debug.Log("Expand Sclale : " + ExpandScaleZ);

        foreach (var o in scaleShrinkObj)
        {
            o.localScale = new Vector3(1f, 1f, shrinkScaleZ);
        }
        foreach (var o in scaleShrinkCvs)
        {
            o.localScale = new Vector3(1f, shrinkScaleZ, 1f);
        }
        foreach (var o in scaleExpandObj)
        {
            o.localScale = new Vector3(1f, 1f, ExpandScaleZ);
        }

        Debug.Log("orthographicSize: " + GetComponent<Camera>().orthographicSize);
        Debug.Log("aspect: " + GetComponent<Camera>().aspect);

        // 9-16 ::: aspect: 0.5624309
        // 9-18 ::: aspect: 0.4994475
    }

}
