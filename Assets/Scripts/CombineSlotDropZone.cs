using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CombineSlotDropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] HotbarController hotbarController;

    List<WordCardView> slottedCards = new List<WordCardView>();

    public IReadOnlyList<WordCardView> SlottedCards => slottedCards;

    public void OnDrop(PointerEventData eventData)
    {
        WordCardView card = eventData.pointerDrag?.GetComponent<WordCardView>();
        if (card == null) return;
        if (slottedCards.Contains(card)) return;

        AddCard(card);
        Debug.Log($"슬롯에 추가: {card.WordData.word}");
    }

    public void AddCard(WordCardView card)
    {
        if (slottedCards.Contains(card)) return;
        card.transform.SetParent(transform);
        card.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        slottedCards.Add(card);
        hotbarController.RearrangeCards();
    }

    public void RemoveCard(WordCardView card)
    {
        if (slottedCards.Contains(card))
        {
            slottedCards.Remove(card);
            hotbarController.RearrangeCards();
            Debug.Log($"슬롯에서 제거: {card.WordData.word}");
        }
    }

    public void Clear()
    {
        slottedCards.Clear();
    }
}