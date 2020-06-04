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



    public int itemGlowNum = 4;

    public int timeBoomBoxActionTime = 60;
    /// <summary>
    /// 바로 아래의 배열변수에서 어떤 것이 타이머에 쓰이는 것인지 숫자로 지정.
    /// </summary>
    public int timerGlowNum = 2;
    public RectTransform[] itemActivate;
    public Color[] itemGlowStartColor;
    float itemGlowStartHue;
    float glowSpendTime;
    float glowSpendTimeMain;

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

        Color.RGBToHSV(itemGlowStartColor[Random.Range(0, itemGlowStartColor.Length)], out float glowH, out float glowS, out float glowV);
        itemGlowStartHue = glowH;
        glowSpendTime = 0;
        glowSpendTimeMain = 0;
    }

    public void Restart()
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
        glowSpendTime = 0;
        glowSpendTimeMain = 0;
        Color.RGBToHSV(itemGlowStartColor[Random.Range(0, itemGlowStartColor.Length)], out float glowH, out float glowS, out float glowV);
        itemGlowStartHue = glowH;
        glowSpendTime = 0;
        glowSpendTimeMain = 0;
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

            glowSpendTimeMain += Time.deltaTime * 0.01f;

            if (itemActivate[0].gameObject.activeSelf == true)
            {
                float cCountGlow = gManager.correctCount / 25f * extendGlow;
                Color gColor = Color.HSVToRGB((itemGlowStartHue + glowSpendTimeMain) % 1f, 0.7f, 1f);

                itemActivate[0].GetComponent<Image>().color = gColor;

                itemActivate[0].offsetMax = new Vector2(8f + cCountGlow * pkf, 8f + cCountGlow * pkf);
                itemActivate[0].offsetMin = new Vector2(-8f + -cCountGlow * pkf, -8f + -cCountGlow * pkf);


                // 타이머 글로우 라인에 붐박스 효과를 적용한다.
                if (gManager.spendTime < timeBoomBoxActionTime)
                {
                    itemActivate[timerGlowNum].offsetMax = new Vector2(8f + extendGlow / 2f * pkf, 8f + extendGlow / 2f * pkf);
                    itemActivate[timerGlowNum].offsetMin = new Vector2(-8f + -extendGlow / 2f * pkf, -8f + -extendGlow / 2f * pkf);
                }
                if(gManager.isCmAllow == true)
                {
                    itemActivate[itemGlowNum].offsetMax = new Vector2(8f + extendGlow / 2f * pkf, 8f + extendGlow / 2f * pkf);
                    itemActivate[itemGlowNum].offsetMin = new Vector2(-8f + -extendGlow / 2f * pkf, -8f + -extendGlow / 2f * pkf);
                }


                //라인에 글로우 효과를 준다.
                glowSpendTime += Time.deltaTime * 0.001f;
                for (int i = 1; i < itemActivate.Length; i++)
                {
                    gColor = Color.HSVToRGB((itemGlowStartHue + (i * 0.0131f) + glowSpendTime) % 1f, 0.7f, 1f);
                    itemActivate[i].GetComponent<Image>().color = gColor;
                }
            }
        }
    }

    public GameManager gManager;

    public float extendGlow;

    float h, s, v;
    float newH;

    public float colorfulWeight = 20f;
    public float ePowerWeight = 10f;
    public float aPowerWeight = 4f;
    public float scaleWeight = 4f;



}
