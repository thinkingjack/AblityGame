using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Pull")]
public class PullAbility : AbilityBase
{
    public override void ActivateAbility(Character character)
    {
        // 능력을 사용한 캐릭터가 바라보는 방향(앞)으로 targetX 계산
        // moveDistance 만큼 앞의 X 좌표를 확인
        float targetX = character.characterTransform.position.x + character.moveDistance;

        // 해당 X 위치에 있는 캐릭터들(앞에 있는 캐릭터들)을 가져옴
        List<Character> frontCharacters = character.turnManager.GetCharactersAtXPosition(targetX);

        // 앞에 있는 캐릭터들을 하나씩 뒤로 당김
        foreach (Character target in frontCharacters)
        {
            // 능력에 의해 캐릭터를 1칸 뒤로 이동
            target.MoveByAbility(-1);
            Debug.Log($"{target.characterName} 능력에 의해 끌려옴");
        }

        // 능력을 사용한 것으로 처리
        character.hasUsedAbility = true;

        // 능력 버튼이 있으면 사용 후 버튼 비활성화
        if (character.abilityButton != null)
            character.abilityButton.gameObject.SetActive(false);
    }
}