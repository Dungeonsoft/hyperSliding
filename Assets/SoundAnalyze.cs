using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeakData
{
    public float amplitude;
}

public class SoundAnalyze : MonoBehaviour
{
    private AudioSource audioSource;



    private List<PeakData> peaks;
    private float[] samples;

    public float threshold = 0.01f;

    const int sampleCount = 64;


    public UnityEngine.UI.Image fxGlow;

    // 백그라운드의 색의 강도를 조절해주기 위해 렌더러를 가져오는 변수.
    public Renderer bgRenderer;

    public List<Transform> scaleT;

    public Transform scaleTY;
    public List<Transform> scaleTYR;
    public List<Transform> scaleTYL;

    public float[] soundWeights;

    public Color imgColor;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Log("Clip Name: " + audioSource.clip.name);

        samples = new float[sampleCount];
        peaks = new List<PeakData>();

        Color.RGBToHSV(imgColor, out h, out s, out v);
        soundWeights =new float[scaleTYR.Count + 1];
    }

    private void OnEnable()
    {
        // 바의 길이를 전부 초기화 하여 준다(Y 높이 0)
        foreach (Transform v in scaleTYR)
        {
            v.localScale = new Vector3(1, 0, 1);
        }
        foreach (Transform v in scaleTYL)
        {
            v.localScale = new Vector3(1, 0, 1);
        }
    }


    void Update()
    {

        audioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular/* .BlackmanHarris*/);
        peaks.Clear(); // 피크 리스트 초기화

        for (int i = 0; i < sampleCount; i++)
        {
            if (samples[i] > threshold)
            {
                peaks.Add(new PeakData
                {
                    amplitude = samples[i]
                });
            }
        }


        if (peaks.Count > 0)
        {
            peaks.Sort((a, b) => -a.amplitude.CompareTo(b.amplitude));

            float peakFreq = peaks[0].amplitude;

            float pkf = peakFreq;


            newH = h - pkf * (colorfulWeight%1);
            //Debug.Log("H value: "+ newH);

            Color newEmisColor =  Color.HSVToRGB(newH, s, v);

            fxGlow.material.SetColor("_EmisColor", newEmisColor);
            fxGlow.material.SetFloat("_EmisPower", pkf * ePowerWeight);
            fxGlow.material.SetFloat("_AlphaPower", pkf * aPowerWeight);
            //bgRenderer.material.SetFloat("_EmisPower", pkf*3f+1f);

            for (int i = soundWeights.Length - 1; i > 0; i--)
            {
                soundWeights[i] = soundWeights[i - 1];
            }
            soundWeights[0] = pkf;

            foreach (var v in scaleT)
            {
                v.localScale = (Vector3.one + Vector3.one * (soundWeights[0] * scaleWeight));
            }

            Color addColor = new Color(0.2f, 0.2f, 0.2f);
            Color barColor = newEmisColor * new Color(1, 1, 1, 0.5f);

            var yValue = pkf * scaleWeight * 5f;
            scaleTY.localScale = (new Vector3(1, yValue* yValue, 1));
            scaleTY.GetChild(0).GetComponent<Image>().color = barColor + addColor;

            for (int i =0; i< scaleTYR.Count; i++)
            {
                yValue = soundWeights[i + 1] * scaleWeight * (5f + i);
                scaleTYR[i].localScale = (new Vector3(1, yValue* yValue, 1));
                scaleTYR[i].GetChild(0).GetComponent<Image>().color = barColor + addColor * (i / 3f);

                scaleTYL[i].localScale = (new Vector3(1, yValue* yValue, 1));
                scaleTYL[i].GetChild(0).GetComponent<Image>().color = barColor + addColor * (i / 3f);
            }

        }
    }

    float h, s, v;
    float newH;

    public float colorfulWeight = 20f;
    public float ePowerWeight = 10f;
    public float aPowerWeight = 4f;
    public float scaleWeight = 4f;



}
