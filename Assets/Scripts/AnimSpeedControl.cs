using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSpeedControl : MonoBehaviour
{
    public Animator anim;
    public float inputBPM = 60.0f;
    float actualBPM;
    public int speedBPM = 1;
    private void OnEnable()
    {
        actualBPM =  inputBPM/ 60.0f;
        anim.speed = actualBPM * speedBPM;
    }
}