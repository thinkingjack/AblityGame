using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTimerManager : MonoBehaviour
{
    public static AbilityTimerManager Instance { get; private set; }
    public Text timerTextUI;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartAbilityTimer(float duration, Action onTimeout, Action onCompleted = null)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        timerTextUI.gameObject.SetActive(true);
        currentCoroutine = StartCoroutine(TimerCoroutine(duration, onTimeout, onCompleted));
    }

    public void StopTimer()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            timerTextUI.gameObject.SetActive(false);
            currentCoroutine = null;
        }
    }

    private IEnumerator TimerCoroutine(float duration, Action onTimeout, Action onCompleted)
    {
        float remaining = duration;

        while (remaining > 0)
        {
            timerTextUI.text = $"능력 사용 시간: {Mathf.CeilToInt(remaining)}초";
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        timerTextUI.text = "자동 선택 중...";
        yield return new WaitForSeconds(0.5f);

        onTimeout?.Invoke();
        onCompleted?.Invoke();
        timerTextUI.gameObject.SetActive(false);
        currentCoroutine = null;
    }
}