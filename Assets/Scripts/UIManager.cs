using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text abilityTimerText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateAbilityTimer(int secondsLeft)
    {
        if (abilityTimerText != null)
        {
            abilityTimerText.text = "능력 선택 남은 시간: " + secondsLeft + "초";
        }
    }

    public void HideAbilityTimer()
    {
        if (abilityTimerText != null)
        {
            abilityTimerText.text = "";
        }
    }
}