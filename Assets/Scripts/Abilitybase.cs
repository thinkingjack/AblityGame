using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbilityBase : ScriptableObject
{
    public string abilityName; // 능력 이름
    public string description; // 설명
    public Sprite icon;

    public abstract void ActivateAbility(Character user);

    public virtual void OnMovedByAbility(Character selfMoved) { }

    public virtual void OnTurnStart(Character self) { } // 추가

    public virtual IEnumerator ActivateAbilityCoroutine(Character self, System.Action onComplete)
    {
        ActivateAbility(self); // 기존 즉시 발동 로직 그대로 사용
        yield return null;     // 코루틴이니까 최소한의 yield 필요
        onComplete?.Invoke();  // 끝났다는 콜백 호출
    }
}