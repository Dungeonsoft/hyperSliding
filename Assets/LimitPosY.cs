using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitPosY : MonoBehaviour
{
    public RectTransform rt;
    // Update is called once per frame
    void Update()
    {
        if(rt.anchoredPosition.y>=2834)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(0,2834),Time.deltaTime*10f);
            //rt.anchoredPosition = new Vector2(0, 2834);
        }
        if(rt.anchoredPosition.y <= 0)
        {
            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y + Time.deltaTime*1000f);
            //rt.anchoredPosition = new Vector2(0, 0);
        }
        ////Debug.Log(rt.rect);
        //Debug.Log(rt.anchoredPosition);
    }
}
