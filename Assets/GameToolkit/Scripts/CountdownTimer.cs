using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    [Header("===== Countdown Settings =====")]
    [SerializeField] private Text countdownText;
    [Range(1, 10)]
    [SerializeField] private int countFrom = 3;
    [Range(0.5f, 3f)]
    [SerializeField] private float secondsPerCount = 1f;
    [SerializeField] private bool useScaleAnimation = true;

    public bool IsCountingDown { get; private set; }
    private Coroutine countdownCoroutine;

    public void StartCountdown(Action onFinished)
    {
        StopCountdown();
        countdownCoroutine = StartCoroutine(CountdownRoutine(onFinished));
    }

    public void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
        IsCountingDown = false;
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    private IEnumerator CountdownRoutine(Action onFinished)
    {
        IsCountingDown = true;
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            for (int i = countFrom; i >= 1; i--)
            {
                countdownText.text = i.ToString();
                if (useScaleAnimation)
                {
                    countdownText.transform.localScale = Vector3.one * 1.5f;
                    float timer = 0f;
                    while (timer < secondsPerCount)
                    {
                        timer += Time.deltaTime;
                        float t = Mathf.Clamp01(timer / secondsPerCount);
                        countdownText.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t);
                        yield return null;
                    }
                }
                else
                {
                    yield return new WaitForSeconds(secondsPerCount);
                }
            }
            countdownText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(countFrom * secondsPerCount);
        }
        IsCountingDown = false;
        countdownCoroutine = null;
        onFinished?.Invoke();
    }
}
