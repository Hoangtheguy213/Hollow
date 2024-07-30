using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartShard : MonoBehaviour
{
    public Image fill;

    public float targetFillAmount;
    public float lerpDuration = 1.5f;
    public float initialFillAmount;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator LerpFill()
    {
        float elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpDuration);

            float lerpedFillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, t);
            fill.fillAmount = lerpedFillAmount;

            yield return null;
        }

        fill.fillAmount = targetFillAmount;

        if(fill.fillAmount == 1)
        {
            Player.Instance.maxHealth++;
            Player.Instance.onHealthChangedCallBack();
            Player.Instance.heartShards = 0;
        }
    }
}
