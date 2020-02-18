using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchFxCon : MonoBehaviour
{

    public AnimationCurve acScale;
    public AnimationCurve acOpaque;
    public Color baseColor;
    public float speedFX;
    
    Transform thisTrans;
    Material thisMat;
    float timeStart;
    float timeNow;

    bool isMovale;

    Action uAction;

    private void Awake()
    {
        thisTrans = this.transform;
        thisMat = GetComponent<Renderer>().material;
        this.gameObject.SetActive(false);
    }

    public void FxCon()
    {
        timeStart = Time.time;
        uAction = StartFX;
    }

    public void MoveNodeCheck(bool b)
    {
        isMovale = b;
    }

    void StartFX()
    {
        timeNow = Time.time - timeStart;
        var eVal = timeNow * speedFX;
        thisMat.SetColor("_EmissionColor", baseColor * acOpaque.Evaluate(eVal));
        thisTrans.localScale = Vector3.one * acScale.Evaluate(eVal);


        if(eVal>=1)
        {
            //Debug.Log("End FX: "+acOpaque.Evaluate(eVal));
            uAction = EndFX;
        }
    }

    void EndFX()
    {
        gameObject.SetActive(false);
        uAction = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(uAction != null)
        {
            uAction();
        }
    }
}
