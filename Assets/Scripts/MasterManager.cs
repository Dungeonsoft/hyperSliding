using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterManager : MonoBehaviour
{
    public GameObject[] firstShowObjs;

    public GameObject[] normalShowObjs;
    
    public bool isAlwaysShowTut = false;

    void Awake()
    {
       string isPlay = PlayerPrefs.GetString("isPlay","false");
        if(isPlay == "false")
        {
            Show(false);
            PlayerPrefs.SetString("isPlay", "true");
        }
        else if(isPlay == "true")
        {
            Show(true);
        }
    }

    void Show(bool isPlay = false)
    {
        if(isAlwaysShowTut == true)
        {
            isPlay = false;
        }

        foreach (var v in firstShowObjs)
        {
            v.SetActive(!isPlay);
        }

        foreach (var v in normalShowObjs)
        {
            v.SetActive(isPlay);
        }

    }

}
