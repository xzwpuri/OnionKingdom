using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HotbarController : MonoBehaviour
{
    [SerializeField] GameObject hotbarRoot;
    [SerializeField] Button rerollButton;
    [SerializeField] Transform cardContainer;
    [SerializeField] WordCardView cardPrefab;
    [SerializeField] CombineSlotDropZone combineSlot;
    [SerializeField] GameObject combineSlotObject;

    [Header("Layout")]
    [SerializeField] float radius = 280f;
    [SerializeField] float angleStart = -60f;
    [SerializeField] float angleEnd = 60f;

    [Header("Time Scale")]
    [SerializeField] float slowTimeScale = 0.3f;

    List<WordCardView> activeCards = new List<WordCardView>();
    List<WordData> selectedWords = new List<WordData>();
    bool isOpen = false;
    bool tabWasDown = false;

    private void Awake()
    {
        hotbarRoot.SetActive(false);
        rerollButton.onClick.AddListener(Reroll);
    }

    private void Update()
    {
        bool tabIsDown = Input.GetKey(KeyCode.Tab);

        if (tabIsDown && !tabWasDown)
            OpenHotbar();

        if (!tabIsDown && tabWasDown)
            CloseHotbar();

        tabWasDown = tabIsDown;
    }

    private void OpenHotbar()
    {
        if (isOpen) return;
        isOpen = true;
        hotbarRoot.SetActive(true);
        combineSlotObject.SetActive(true); // 추가
        Time.timeScale = slowTimeScale;
        List<WordData> hotbarWords = DeckManager.Instance.OpenHotbar();
        SpawnCards(hotbarWords);
    }

    private void CloseHotbar()
    {
        if (!isOpen) return;
        isOpen = false;
        hotbarRoot.SetActive(false);
        combineSlotObject.SetActive(false);
        Time.timeScale = 1f;

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);
        activeCards.Clear();

        DeckManager.Instance.CloseHotbar();

        // 슬롯 순서 그대로 단어 목록 가져오기
        List<WordData> orderedWords = new List<WordData>();
        foreach (var card in combineSlot.SlottedCards)
            orderedWords.Add(card.WordData);

        if (orderedWords.Count > 0)
        {
            WeaponData weapon = WeaponBuilder.TryBuild(orderedWords);
            if (weapon != null)
            {
                WeaponManager.Instance.EquipWeapon(weapon);
            }
            else
            {
                Debug.Log("무기 생성 실패: 문법 오류");
            }
        }

        foreach (Transform child in combineSlot.transform)
            Destroy(child.gameObject);
        combineSlot.Clear();
    }

    private void Reroll()
    {
        List<WordData> newWords = DeckManager.Instance.RerollHotbar();
        selectedWords.Clear();
        SpawnCards(newWords);
    }

    private void SpawnCards(List<WordData> words)
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);
        activeCards.Clear();

        if (words.Count == 0) return;

        float angleStep = words.Count > 1
            ? (angleEnd - angleStart) / (words.Count - 1)
            : 0f;

        for (int i = 0; i < words.Count; i++)
        {
            float angle = angleStart + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius
            );

            WordCardView card = Instantiate(cardPrefab, cardContainer);
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);
            card.Setup(words[i]);
            card.OnCardClicked += OnCardSelected;
            activeCards.Add(card);
        }
    }

    private void OnCardSelected(WordCardView card)
    {
        if (selectedWords.Contains(card.WordData))
        {
            selectedWords.Remove(card.WordData);
            Debug.Log($"선택 해제: {card.WordData.word}");
        }
        else
        {
            selectedWords.Add(card.WordData);
            Debug.Log($"선택: {card.WordData.word} | 현재 조합: {string.Join(" + ", selectedWords.ConvertAll(w => w.word))}");
        }
    }

    public void RearrangeCards()
    {
        List<WordCardView> remainingCards = new List<WordCardView>();
        foreach (Transform child in cardContainer)
        {
            WordCardView card = child.GetComponent<WordCardView>();
            if (card != null)
                remainingCards.Add(card);
        }

        if (remainingCards.Count == 0) return;

        float angleStep = remainingCards.Count > 1
            ? (angleEnd - angleStart) / (remainingCards.Count - 1)
            : 0f;

        for (int i = 0; i < remainingCards.Count; i++)
        {
            float angle = remainingCards.Count > 1
                ? angleStart + angleStep * i
                : (angleStart + angleEnd) / 2f;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius
            );

            RectTransform rect = remainingCards[i].GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero; // 먼저 초기화
            rect.anchoredPosition = pos;
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public void AddCardToHotbar(WordData wordData)
    {
        // 현재 핫바 카드 목록 가져오기
        List<WordData> currentWords = new List<WordData>();
        foreach (Transform child in cardContainer)
        {
            WordCardView c = child.GetComponent<WordCardView>();
            if (c != null)
                currentWords.Add(c.WordData);
        }
        currentWords.Add(wordData);
        SpawnCards(currentWords);
    }
}