using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarDropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] HotbarController hotbarController;

    public void OnDrop(PointerEventData eventData)
    {
        WordCardView card = eventData.pointerDrag?.GetComponent<WordCardView>();
        if (card == null) return;

        // 기존 카드 제거하고 새로 생성
        Destroy(card.gameObject);
        hotbarController.AddCardToHotbar(card.WordData);
    }
}