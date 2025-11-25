using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public bool isAbilityDisabledThisTurn = false;

    public GameObject characterUI; // 인스펙터에서 각자의 UI 패널 연결
    public string characterName;
    public int position = 0;
    public List<int> moveCards = new List<int> { 1, 2, 3, 4 };

    public GameObject abilityPanel; // 방향 선택 UI (밀기용)
    public Button forwardButton;
    public Button backwardButton;

    public GameObject cardPrefab;
    public Transform cardParent;
    public Transform characterTransform;
    public float moveDistance;

    public TurnManager turnManager;
    private GameObject buttonPanel;

    public bool hasUsedAbility = false;
    public Button abilityButton;

    // 능력 관련
    public AbilityBase assignedAbility; // 스크립터블 오브젝트 기반 능력
    public int lastUsedCard = -1;       // 마지막 사용 카드
    public bool enableCardBonus = false; // 리셋 시 카드 보너스 활성 여부

    void Start()
    {
        ResetCards();
        DisplayMoveCards();
        SetupAbilityButton();
    }
    
        public void AssignAbility(AbilityBase ability)
    {
        assignedAbility = ability;
    }
    public void SetButtonPanel(GameObject panel)
    {
        buttonPanel = panel;
    }

    public void SetTurnManager(TurnManager manager)
    {
        turnManager = manager;
    }

    public void SetActiveTurn(bool isActive)
    {

        if (cardParent != null)
            cardParent.gameObject.SetActive(isActive);

        if (buttonPanel != null)
            buttonPanel.SetActive(isActive);

        if (abilityButton != null)
            abilityButton.gameObject.SetActive(isActive && !hasUsedAbility);

        if (characterUI != null)
        {
            characterUI.SetActive(isActive);
        }

        if (isActive)
        {
            DisplayMoveCards();
            turnManager.StartTurnTimer(this);
        }
    }


    public void Move(int moveValue)
    {
        if (!turnManager.IsCurrentTurn(this))
        {
            Debug.LogWarning($"{characterName}의 턴이 아님");
            return;
        }

        position += moveValue;
        Vector3 newPosition = characterTransform.position + new Vector3(moveValue * moveDistance, 0, 0);
        characterTransform.position = newPosition;

        lastUsedCard = moveValue;
        moveCards.Remove(moveValue);

        if (moveCards.Count == 0)
        {
            ResetCards();
        }

        DisplayMoveCards();

        turnManager.CheckFinishLine(this);
        SetActiveTurn(false);
        turnManager.NextTurn();

    }

    //능력에 의한 이동
    public void MoveByAbility(int moveValue)
    {
        
        position += moveValue;
        Vector3 newPosition = characterTransform.position + new Vector3(moveValue * moveDistance, 0, 0);
        characterTransform.position = newPosition;

        assignedAbility?.OnMovedByAbility(this);
    }



    private void ResetCards()
    {
        moveCards.Clear();
        moveCards.AddRange(new int[] { 1, 2, 3, 4 });

        if (enableCardBonus && lastUsedCard != -1)
        {
            moveCards.Add(lastUsedCard);
            Debug.Log($"{characterName} 카드 리셋 시 {lastUsedCard} 보너스 카드 추가");
        }
    }

    public void DisplayMoveCards()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }

        foreach (int moveValue in moveCards)
        {
            GameObject card = Instantiate(cardPrefab, cardParent);
            card.GetComponentInChildren<Text>().text = moveValue.ToString();


            int valueCopy = moveValue; // 복사 변수
            Button button = card.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => Move(valueCopy));


        }
    }
    
    public void SetupAbilityButton()
    {
        if (abilityPanel != null)
            abilityPanel.SetActive(false);

        if (forwardButton != null)
        {
            forwardButton.onClick.RemoveAllListeners();
            forwardButton.onClick.AddListener(() => PushCharacters(1));
        }

        if (backwardButton != null)
        {
            backwardButton.onClick.RemoveAllListeners();
            backwardButton.onClick.AddListener(() => PushCharacters(-1));
        }

        if (abilityButton != null)
        {
            abilityButton.onClick.RemoveAllListeners();
            abilityButton.onClick.AddListener(UseAbility);
        }
    }
    public void UseAbility()
    {
        if (hasUsedAbility)
        {
            Debug.Log($"{characterName}은 이미 능력을 사용했습니다.");
            return;
        }

        if (isAbilityDisabledThisTurn)
        {
            Debug.Log($"{characterName}의 능력은 이번 턴 무력화되어 사용할 수 없습니다!");
            return;
        }

        if (assignedAbility != null)
        {
            // 능력 실행을 코루틴으로 처리 (타이머 포함)
            StartCoroutine(assignedAbility.ActivateAbilityCoroutine(this, () =>
            {
                hasUsedAbility = true;
                abilityButton.gameObject.SetActive(false);

                // 능력 후 이동 카드 표시 (제어권 넘겨주는 방식)
                DisplayMoveCards();
            }));
        }
        else
        {
            Debug.LogWarning($"{characterName}에게 할당된 능력이 없습니다!");
        }
    }

    // 밀기 전용: UI 활성화
    public void PreparePushAbility()
    {
        float currentX = characterTransform.position.x;
        List<Character> charactersOnSameX = turnManager.GetCharactersAtXPosition(currentX);
        charactersOnSameX.Remove(this);

        if (charactersOnSameX.Count > 0)
        {
            abilityPanel.SetActive(true);
        }
        else
        {
            Debug.Log("같은 위치에 다른 캐릭터 없음");
        }
    }

    // 밀기 실행
    public void PushCharacters(int direction)
    {
        float currentX = characterTransform.position.x;
        List<Character> targets = turnManager.GetCharactersAtXPosition(currentX);
        targets.Remove(this);

        foreach (Character target in targets)
        {
            target.position += direction;
            target.characterTransform.position += new Vector3(direction * moveDistance, 0, 0);
            Debug.Log($"{target.characterName}이(가) {direction}칸 밀려남");
        }

        hasUsedAbility = true;
        abilityPanel.SetActive(false);
        if (abilityButton != null)
            abilityButton.gameObject.SetActive(false);
    }

    // 끌어오기 직접 실행 (스크립터블에서 사용)
    public void PullFrontCharacters()
    {
        float targetX = characterTransform.position.x + moveDistance;
        List<Character> frontCharacters = turnManager.GetCharactersAtXPosition(targetX);

        foreach (Character target in frontCharacters)
        {
            target.position -= 1;
            Vector3 newPos = new Vector3(characterTransform.position.x, target.characterTransform.position.y, target.characterTransform.position.z);
            target.characterTransform.position = newPos;
            Debug.Log($"{target.characterName}이(가) 끌려옴");
        }
    }
}