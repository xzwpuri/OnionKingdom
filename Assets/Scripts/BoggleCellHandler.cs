using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoggleCellHandler : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    [SerializeField] Image _backgroundImage;
    [SerializeField] TextMeshProUGUI _cellLetter;

    [SerializeField] Color normalColor;
    [SerializeField] Color selectedColor;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _backgroundImage.color = selectedColor;
        Debug.Log("Enter");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _backgroundImage.color = selectedColor;
        Debug.Log("Down");
    }
}
