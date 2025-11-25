using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PushAbility", menuName = "Ability/Push")]
public class PushAbility : AbilityBase
{
    public override void ActivateAbility(Character user)
    {
        float currentX = user.characterTransform.position.x;

        // 같은 위치에 있는 다른 캐릭터들을 찾는다
        var targets = user.turnManager.GetCharactersAtXPosition(currentX);
        targets.Remove(user);

        if (targets.Count == 0)
        {
            Debug.LogWarning("같은 칸에 있는 캐릭터가 없습니다!");
            return;
        }

        // 캐릭터의 abilityPanel UI를 띄워서 사용자에게 방향 선택을 맡긴다
        if (user.abilityPanel != null)
        {
            user.abilityPanel.SetActive(true);

            // 버튼 리스너 초기화 및 설정
            user.forwardButton.onClick.RemoveAllListeners();
            user.forwardButton.onClick.AddListener(() =>
            {
                ApplyPush(targets, 1, user);
                user.abilityPanel.SetActive(false);
            });

            user.backwardButton.onClick.RemoveAllListeners();
            user.backwardButton.onClick.AddListener(() =>
            {
                ApplyPush(targets, -1, user);
                user.abilityPanel.SetActive(false);
            });
        }
    }

    private void ApplyPush(List<Character> targets, int direction, Character user)
    {
        //foreach (var target in targets)
        //{
        //    target.position += direction;
        //    Vector3 newPosition = target.characterTransform.position + new Vector3(direction * user.moveDistance, 0, 0);
        //    target.characterTransform.position = newPosition;

        //    Debug.Log($"{target.characterName}이(가) {direction}칸 밀려남.");
        //}
        foreach (var target in targets)
        {
            // 직접 위치 조작하지 않고 MoveByAbility로 이동 처리
            target.MoveByAbility(direction);
            Debug.Log($"{target.characterName}이(가) {direction}칸 밀려남.");
        }

        user.hasUsedAbility = true;
        if (user.abilityButton != null)
            user.abilityButton.gameObject.SetActive(false);
    }
}