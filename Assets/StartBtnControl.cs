using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartBtnControl : MonoBehaviour
{
    public Image img;

    private void Awake()
    {
        img.raycastTarget = false;
    }
    void OnEnable()
    {
        img.raycastTarget = false;
        StartCoroutine(WaitStartAction());
    }

    IEnumerator WaitStartAction()
    {
        yield return new WaitForSeconds(3f);
        img.raycastTarget = true;
    }

    private void OnDisable()
    {
        img.raycastTarget = false;
    }
}
