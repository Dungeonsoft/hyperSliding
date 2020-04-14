using System.Collections;
using System.Collections.Generic;
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

    private void OnEnable()
    {
        foreach (var v in sData)
        {
            v.data.SetActive(false);
        }

        StartCoroutine(IEShowSequence());
    }

    IEnumerator IEShowSequence()
    {
        foreach (var v in sData)
        {
            yield return new WaitForSeconds(v.delayTime);
            v.data.SetActive(true);
        }
    }

    private void OnDisable()
    {
        foreach (var v in sData)
        {
            v.data.SetActive(false);
        }
    }
}
