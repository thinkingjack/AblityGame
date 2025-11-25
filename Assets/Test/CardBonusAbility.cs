using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Card Bonus On Reset")]
public class CardBonusAbility : AbilityBase
{
    public override void ActivateAbility(Character character)
    {
        character.enableCardBonus = true; // 캐릭터가 이 기능을 가진 것으로 표시
        character.hasUsedAbility = true;
        if (character.abilityButton != null) character.abilityButton.gameObject.SetActive(false);

        Debug.Log($"{character.characterName} 카드 리셋 시 마지막 사용 카드 추가 능력 획득!");
    }
}
