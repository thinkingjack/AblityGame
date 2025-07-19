using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/DisableOtherAbility")]
public class DisableOtherAbility : AbilityBase
{

    public override void ActivateAbility(Character user)
    {
        // 직접 호출되는 능력이 아니므로 비워둡니다.
    }

    public override void OnTurnStart(Character self)
    {
        List<Character> targets = new List<Character>();
        foreach (var character in TurnManager.Instance.characters)
        {
            if (character != self)
                targets.Add(character);
        }
        TurnManager.Instance.StartAbilityTimer((selectedCharacter) =>
        {
            if (selectedCharacter == null)
            {
                // 시간 초과 시 무작위 선택
                Character randomTarget = targets[Random.Range(0, targets.Count)];
                randomTarget.isAbilityDisabledThisTurn = true;
                Debug.Log($"{randomTarget.characterName}의 능력이 이번 턴 무작위로 무력화됨!");
            }
            else
            {
                selectedCharacter.isAbilityDisabledThisTurn = true;
                Debug.Log($"{selectedCharacter.characterName}의 능력이 이번 턴 무력화됨!");
            }

            self.DisplayMoveCards();
        }, targets);
    }

}