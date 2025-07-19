using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Card Destroy Ability")]
public class CardEraseAbility : AbilityBase
{

    private bool hasActivated = false;

    public override void ActivateAbility(Character user)
    {
        // 이 능력은 직접 사용되는 게 아님
    }

    public override void OnMovedByAbility(Character selfMoved)
    {
        Debug.Log("dd");
        if (hasActivated) return;
        hasActivated = true;

        // 자기 자신 제외하고 선택할 캐릭터들 리스트 만들기
        List<Character> selectableTargets = new List<Character>();
        foreach (var character in TurnManager.Instance.characters)
        {
            if (character != selfMoved)
                selectableTargets.Add(character);
            Debug.Log($"선택 대상: {character.characterName}");
        }

        // 선택 UI 띄우기

        Debug.Log("CharacterSelector UI 띄우는 중...");
        CharacterSelector.Instance.Show(selectableTargets, (Character selected) =>
        {
            Debug.Log($"선택된 캐릭터: {selected?.characterName}");
            if (selected == null || selected.moveCards.Count <= 1) return;

            // 1장 남기고 제거
            int keepIndex = Random.Range(0, selected.moveCards.Count);
            int keepCard = selected.moveCards[keepIndex];

            selected.moveCards.Clear();
            selected.moveCards.Add(keepCard);

            Debug.Log($"[{selfMoved.characterName}]의 능력 발동! {selected.characterName}의 카드 1장만 남김.");

            // 선택이 완료되면, CharacterSelector UI 숨기기
            CharacterSelector.Instance.gameObject.SetActive(false);

        });
    }
}