using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Ability/Pull")]
public class PullAbility : AbilityBase
{
    public override void ActivateAbility(Character character)
    {
        float targetX = character.characterTransform.position.x + character.moveDistance;
        List<Character> frontCharacters = character.turnManager.GetCharactersAtXPosition(targetX);

        foreach (Character target in frontCharacters)
        {
            // 능력에 의한 이동 처리로 변경
            target.MoveByAbility(-1); // 1칸 뒤로 이동
            Debug.Log($"{target.characterName} 능력에 의해 끌려옴");
        }

        character.hasUsedAbility = true;
        if (character.abilityButton != null) character.abilityButton.gameObject.SetActive(false);
    }
}