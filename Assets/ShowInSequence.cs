using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ShowSequenceData
{
    public float delayTime;
    public GameObject data;
}

public class ShowInSequence : MonoBehaviour
{
    [SerializeField]
    public List<ShowSequenceData> sData;

    public TextMeshProUGUI finalScoreText;

    public int finalScore;

    bool isStartRollingScore = false;
    float spendTime = 0f;

    /// <summary>
    /// 성공 결과창에서 베스트 스코어는 늦게 보이니 사라질때도 감추어줘야 되서 만든것.
    /// 실패 결과창에서는 쓸모가 없으니 그곳에서는 이곳에 아무것도 지정하면 안됨.
    /// </summary>
    public GameObject bestScore;

    private void OnEnable()
    {
        foreach (var v in sData)
        {
            v.data.SetActive(false);
        }

        finalScoreText.text = "00000000000";
        isStartRollingScore = false;
        spendTime = 0f;
        StartCoroutine(IEShowSequence());
    }

    IEnumerator IEShowSequence()
    {
        foreach (var v in sData)
        {
            //Debug.Log("개별점수_딜레이타임: " + v.delayTime + "현재시간1: " + Time.time);
            yield return new WaitForSeconds(v.delayTime);
            //Debug.Log("개별점수_딜레이타임: " + v.delayTime + "현재시간2: " + Time.time);
            v.data.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        isStartRollingScore = true;

    }

    private void OnDisable()
    {
        foreach (var v in sData)
        {
            v.data.SetActive(false);
        }
        isStartRollingScore = false;
        spendTime = 0f;
        
        if(bestScore != null)
        {
            bestScore.SetActive(false);
        }
    }

    private void Update()
    {
        if(isStartRollingScore == true)
        {
            spendTime += Time.deltaTime;
            finalScoreText.text = Mathf.CeilToInt(finalScore * spendTime).ToString("00000000000");
            if (spendTime>= 1f)
            {
                isStartRollingScore = false;
                spendTime = 0f;
            }
        }
    }
}
