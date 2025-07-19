using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector Instance;

    public GameObject buttonPrefab;
    public Transform buttonContainer; // 버튼들이 들어갈 곳
    private System.Action<Character> onSelectedCallback;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gameObject.SetActive(false);
    }

    public void Show(List<Character> characters, System.Action<Character> onSelected)
    {
        ClearButtons();
        onSelectedCallback = onSelected;

        foreach (var character in characters)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            Text text = btnObj.GetComponentInChildren<Text>();
            text.text = character.characterName;

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => SelectCharacter(character));
        }

        gameObject.SetActive(true);
    }

    private void SelectCharacter(Character selected)
    {
        onSelectedCallback?.Invoke(selected);
        gameObject.SetActive(false);
        ClearButtons();
    }

    private void ClearButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}