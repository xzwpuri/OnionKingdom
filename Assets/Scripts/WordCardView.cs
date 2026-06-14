using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WordCardView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI wordText;
    [SerializeField] Image backgroundImage;
    [SerializeField] Color verbColor;
    [SerializeField] Color verbTextColor;
    [SerializeField] Color defaultColor;
    [SerializeField] Color defaultTextColor;

    WordData wordData;
    public Action<WordCardView> OnCardClicked;

    public WordData WordData => wordData;

    public void Setup(WordData data)
    {
        wordData = data;
        wordText.text = data.word;
        typeText.text = data.type.ToString();

        if (data.type == WordType.Verb)
        {
            backgroundImage.color = verbColor;
            wordText.color = verbTextColor;
            typeText.color = verbTextColor;
        }
        else
        {
            backgroundImage.color = defaultColor;
            wordText.color = defaultTextColor;
            typeText.color = defaultTextColor;
        }

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => OnCardClicked?.Invoke(this));
    }
}