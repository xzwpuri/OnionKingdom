using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoggleCellView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    [SerializeField] Image _backgroundImage;
    [SerializeField] TextMeshProUGUI _cellLetter;

    public int Col { get; set; }
    public int Row { get; set; }
    public bool IsSelected { get; set; }

    public Action<BoggleCellView> CellPointerEnter;
    public Action<BoggleCellView> CellPointerDown;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CellPointerEnter?.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CellPointerDown?.Invoke(this);
    }

    public void SetLetter(char letter)
    {
        _cellLetter.text = letter.ToString();
    }

    public void SetCell(int col, int row, char letter)
    {
        Col = col;
        Row = row;
        SetLetter(letter);
        IsSelected = false;
    }

    public void SetSelected(Color color)
    {
        IsSelected = true;
        _backgroundImage.color = color;
    }

    public void SetColor(Color color)
    {
        _backgroundImage.color = color;
    }

    public void UnSelected(Color normalColor)
    {
        IsSelected = false;
        _backgroundImage.color = normalColor;
    }
}