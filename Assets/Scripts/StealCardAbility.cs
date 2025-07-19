using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/StealCardAbility")]
public class StealCardAbility : AbilityBase
{
    private bool hasActivated = false;

    public override void ActivateAbility(Character user)
    {
        // 직접 사용하는 능력은 아님
    }

    public override void OnMovedByAbility(Character selfMoved)
    {
        if (hasActivated) return;
        hasActivated = true;

        List<Character> targets = new List<Character>();
        foreach (var character in TurnManager.Instance.characters)
        {
            if (character != selfMoved && character.moveCards.Count > 0)
                targets.Add(character);
        }

        if (targets.Count == 0)
        {
            Debug.Log("훔칠 수 있는 캐릭터가 없음");
            return;
        }

        CharacterSelector.Instance.Show(targets, (Character selected) =>
        {
            if (selected == null || selected.moveCards.Count == 0) return;

            int stealIndex = Random.Range(0, selected.moveCards.Count);
            int stolenCard = selected.moveCards[stealIndex];

            selected.moveCards.RemoveAt(stealIndex);
            selfMoved.moveCards.Add(stolenCard);

            Debug.Log($"{selfMoved.characterName}이(가) {selected.characterName}에게서 {stolenCard} 카드를 훔쳤습니다.");

            // 카드 UI 갱신
            selected.DisplayMoveCards();
            selfMoved.DisplayMoveCards();
        });
    }
}