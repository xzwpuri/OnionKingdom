using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Canvas canvas;

    Vector2 originalPosition;
    Transform originalParent;
    Quaternion originalRotation;
    CombineSlotDropZone slotZone;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalRotation = rectTransform.localRotation;

        // 슬롯에서 드래그하는 경우 체크
        slotZone = originalParent.GetComponent<CombineSlotDropZone>();
        if (slotZone != null)
            slotZone.RemoveCard(GetComponent<WordCardView>());

        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
        rectTransform.localRotation = Quaternion.identity;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // 슬롯이나 핫바에 드롭 안 됐으면
        if (transform.parent == canvas.transform)
        {
            if (slotZone != null)
            {
                // 슬롯에서 뺐다가 아무데도 못 놓으면 슬롯으로 복귀
                slotZone.AddCard(GetComponent<WordCardView>());
            }
            else
            {
                // 핫바에서 뺐다가 아무데도 못 놓으면 핫바로 복귀
                transform.SetParent(originalParent);
                rectTransform.anchoredPosition = originalPosition;
                rectTransform.localRotation = originalRotation;
            }
        }
    }
}