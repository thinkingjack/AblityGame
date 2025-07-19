using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public List<AbilityBase> allAbilities; // 가능한 능력들의 리스트
    public List<Character> characters;
    private int currentTurnIndex = 0;
    public GameObject buttonPanel;
    public float finishLineX = 8f; // 결승선의 X 좌표
    private List<Character> finishOrder = new List<Character>(); // 도착한 순서 리스트
    public Text resultText; // 게임 결과를 표시할 UI Text
    public Text timerText;
    private float turnTimeLimit = 10f;
    private Coroutine turnTimerCoroutine;
    public static TurnManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        foreach (Character character in characters)
        {
            character.SetButtonPanel(buttonPanel);
            character.SetActiveTurn(false);
            character.SetTurnManager(this);

            // 능력 무작위 할당
            if (allAbilities != null && allAbilities.Count > 0)
            {
                AbilityBase randomAbility = Instantiate(allAbilities[Random.Range(0, allAbilities.Count)]);
                character.AssignAbility(randomAbility);
            }
        }
        if (characters == null || characters.Count == 0)
        {
            Debug.LogError("TurnManager: 캐릭터 리스트가 비어 있습니다!");
            return;
        }



        foreach (Character character in characters)
        {
            character.SetButtonPanel(buttonPanel);
            character.SetActiveTurn(false);
            character.SetTurnManager(this);
        }

        characters[currentTurnIndex].SetActiveTurn(true);
        if (buttonPanel != null) buttonPanel.SetActive(true);
        if (resultText != null) resultText.gameObject.SetActive(false);// 결과 텍스트 비활성화

    }
    // x축 위치가 같은 캐릭터들을 반환
    public List<Character> GetCharactersAtXPosition(float xPosition)
    {
        List<Character> charactersAtXPosition = new List<Character>();

        foreach (Character character in characters)
        {
            // x축 위치가 정확히 같은 캐릭터 탐지
            if (Mathf.Approximately(character.characterTransform.position.x, xPosition))
            {
                charactersAtXPosition.Add(character);
            }
        }
        return charactersAtXPosition;
    }

    public void NextTurn()
    {
        if (finishOrder.Count >= 2)
        {
            return; // 게임 종료 시 추가 턴 방지
        }

        currentTurnIndex = (currentTurnIndex + 1) % characters.Count;
        Debug.Log("다음 턴: " + characters[currentTurnIndex].characterName);

        StartCoroutine(DelayedUpdateTurn());
    }

    private IEnumerator DelayedUpdateTurn()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateTurn();

        if (buttonPanel != null)
        {
            buttonPanel.SetActive(true);
        }
    }

    public void StartAbilityTimer(System.Action<Character> onComplete, List<Character> candidates)
    {
        StartCoroutine(AbilitySelectionTimerCoroutine(onComplete, candidates));
    }

    private IEnumerator AbilitySelectionTimerCoroutine(System.Action<Character> onComplete, List<Character> candidates)
    {
        float timeLimit = 10f;
        float remaining = timeLimit;

        // 선택 UI + 타이머 시작
        CharacterSelector.Instance.Show(candidates, (selected) =>
        {
            StopCoroutine("AbilitySelectionTimerCoroutine"); // 중복 방지
            CharacterSelector.Instance.Hide();
            onComplete?.Invoke(selected);
        });

        while (remaining > 0)
        {
            UIManager.Instance.UpdateAbilityTimer(Mathf.CeilToInt(remaining)); // 타이머 표시
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        // 시간 초과 처리
        CharacterSelector.Instance.Hide();
        onComplete?.Invoke(null);
    }
    public void UpdateTurn()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            Character character = characters[i];

            // 자신의 턴이면 활성화
            bool isActive = (i == currentTurnIndex);
            character.SetActiveTurn(isActive);

            if (isActive)
            {
                //턴 시작 시 능력 자동 발동
                character.assignedAbility?.OnTurnStart(character);
            }
        }
    }

    public bool IsCurrentTurn(Character character)
    {
        return characters[currentTurnIndex] == character;
    }

    public void CheckFinishLine(Character character)
    {
        if (character.characterTransform.position.x >= finishLineX && !finishOrder.Contains(character))
        {
            finishOrder.Add(character);
            Debug.Log(character.characterName + " 결승선 도착! 현재 순위: " + finishOrder.Count);

            if (finishOrder.Count == 2)
            {
                // 두 번째 도착한 캐릭터가 승리!
                resultText.gameObject.SetActive(true); // 승리 텍스트 활성화
                resultText.text = "승리: " + character.characterName;
                Debug.Log("게임 종료! 승자: " + character.characterName);
            }
        }
    }

    public void StartTurnTimer(Character character)
    {
        if (turnTimerCoroutine != null)
            StopCoroutine(turnTimerCoroutine);

        timerText.gameObject.SetActive(true); // UI 강제 활성화
        turnTimerCoroutine = StartCoroutine(TurnTimerCoroutine(character));
    }


    
    private IEnumerator TurnTimerCoroutine(Character character)
    {
        float remainingTime = turnTimeLimit;
        while (remainingTime > 0)
        {
            timerText.text = "남은 시간: " + Mathf.CeilToInt(remainingTime);
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        timerText.text = "자동 선택중";
        int randomMove = character.moveCards[Random.Range(0, character.moveCards.Count)];
        character.Move(randomMove);
    }
}