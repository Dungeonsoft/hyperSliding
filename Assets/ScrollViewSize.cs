using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewSize : MonoBehaviour
{
    private void Awake()
    {
        var w =  Screen.width / 720f;
        var h = Screen.height / w;

        GetComponent<RectTransform>().sizeDelta = new Vector2(720, h);
    }

}
