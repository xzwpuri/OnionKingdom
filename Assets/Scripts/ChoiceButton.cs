using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI choiceText;
    [SerializeField] Button button;

    public void Setup(string text, Action onClick)
    {
        choiceText.text = text;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}