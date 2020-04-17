using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAnimFx : MonoBehaviour
{

    float AlphaValue = 0;
    Material mat01;
    Material mat02;
    Transform thisT;
    Transform childT;
    public string propertyName01;
    public string propertyName02;
    public AnimationCurve acAlpha01;
    public AnimationCurve acScale02;
    public Color pColor01;
    public Color pColor02;
    public float pValue;
    float spendtime;
    public float speed = 1f;
    public float maxAddScale = 0.2f;
    public bool isHide = false;
    public Transform parent;

    private void OnEnable()
    {
        AlphaValue = 0;
        spendtime = 0;
        thisT = this.transform;
        childT = thisT.GetChild(0);
        mat01 = thisT.GetComponent<Renderer>().material;
        mat02 = childT.GetComponent<Renderer>().material;
    }

    void Update()
    {
        //Debug.Log("이펙트 위치: "+this.transform.position);
        spendtime += Time.deltaTime * speed;
        mat01.SetColor(propertyName01, pColor01 * acAlpha01.Evaluate(spendtime));
        mat02.SetColor(propertyName02, pColor02 * acAlpha01.Evaluate(spendtime));
        thisT.localScale = Vector3.one + Vector3.one * acScale02.Evaluate(spendtime) * maxAddScale;

        if (spendtime >= 1)
        {
            spendtime = 1;
            if (isHide == true)
                this.gameObject.SetActive(false);
        }
    }
}
