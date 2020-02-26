using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgAnim : MonoBehaviour
{
    public Color baseClr;
    float baseH;
    public float intervalBtClrs;

    public Renderer[] rMat;

    float outH, outS, outV;

    /// <summary>
    /// 색의 변화 속도를 조절한다. 1을 1초의 기준으로 잡고 값이 크면 더 빠르게 작으면 느리게
    /// </summary>
    public float colorChangeSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("intervalBtClrs" + intervalBtClrs / 360f);
        Color.RGBToHSV(baseClr, out outH, out outS, out outV);
        baseH = outH;
        Debug.Log("outH: "+ outH + "outS: " + outS + "outV: " + outV);
        
        for(int i =0; i<rMat.Length; i++)
        {
            var newH = (outH + (intervalBtClrs/360f) * i);
            //Debug.Log("newH: "+ newH);
            bool isCheck = true;
            while (isCheck)
            {
                if (newH > 1f) newH -= 1f;
                else isCheck = false;
            }
            rMat[i].material.SetColor("_EmisColor", Color.HSVToRGB(newH, outS, outV));

            float hh, ss, vv = 0;
            Color cc =rMat[i].material.GetColor("_EmisColor");
            Color.RGBToHSV(cc, out hh, out ss, out vv);

            //Debug.Log("Color: " + i + " : " +hh+" :: "+ss+ " :: " + vv); ;
        }

        oldBaseClr = baseClr;
    }

    Color oldBaseClr;
    void Update()
    {
        baseH += Time.deltaTime * colorChangeSpeed; 
        // if (baseClr == oldBaseClr) return;

        Color.RGBToHSV(baseClr, out outH, out outS, out outV);

        outH = baseH;

        for (int i = 0; i < rMat.Length; i++)
        {
            var newH = (outH + (intervalBtClrs / 360f) * i);
            bool isCheck = true;
            while (isCheck)
            {
                if (newH > 1f) newH -= 1f;
                else isCheck = false;
            }
            //Debug.Log("newH: " + newH);
            rMat[i].material.SetColor("_EmisColor", Color.HSVToRGB(newH, outS, outV));
        }

        oldBaseClr = baseClr;
    }
}
