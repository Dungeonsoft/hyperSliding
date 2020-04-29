using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using random = UnityEngine.Random;

public class BgAnim : MonoBehaviour
{
    public Color baseClr;
    float baseH;
    float baseS;
    float baseV;
    public float intervalBtClrs;

    public Renderer rBase;
    public Renderer[] rMat;

    float outH, outS, outV;

    public List<Texture> gradationImgs;
    Texture gradationImgSel;
    /// <summary>
    /// 색의 변화 속도를 조절한다. 1을 1초의 기준으로 잡고 값이 크면 더 빠르게 작으면 느리게
    /// </summary>
    public float colorChangeSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("intervalBtClrs" + intervalBtClrs / 360f);
        Color.RGBToHSV(baseClr, out outH, out outS, out outV);
        baseH = outH;
        baseS = outS;
        baseV = 0;
        
    }
    private void OnEnable()
    {
        baseV = 0;

        var num = random.Range(0, gradationImgs.Count);
        gradationImgSel = gradationImgs[num];

        for (int i = 0; i < rMat.Length; i++)
        {
            var newH = (outH + (intervalBtClrs / 360f) * i);
            //Debug.Log("newH: "+ newH);
            bool isCheck = true;
            while (isCheck)
            {
                if (newH > 1f) newH -= 1f;
                else isCheck = false;
            }
            rMat[i].material.SetColor("_EmisColor", Color.HSVToRGB(newH, outS, outV));
            rMat[i].material.SetTexture("_GradTex", gradationImgSel);
            rMat[i].material.SetTextureOffset("_GradTex", new Vector2(0,0));
        }

    }
    void Update()
    {
        baseH += Time.deltaTime * colorChangeSpeed;

        if(baseV <1)
        {
            baseV += Time.deltaTime*0.5f;
        }
        

        float intervalBtClrsFloat = intervalBtClrs / 360f;
        float newH = 0;

        for (int i = 0; i < rMat.Length; i++)
        {
            newH = (baseH + intervalBtClrsFloat * i);
            bool isCheck = true;
            while (isCheck)
            {
                if (newH > 1f) newH -= 1f;
                else isCheck = false;
            }
            rMat[i].material.SetColor("_EmisColor", Color.HSVToRGB(newH, baseS/2, baseV));
            rMat[i].material.SetTextureOffset("_GradTex", new Vector2(0, baseH*3f));
        }
    }
}
