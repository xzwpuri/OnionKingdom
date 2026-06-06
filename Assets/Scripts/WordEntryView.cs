using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordEntryView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI wordText;
    [SerializeField] Image backgroundImage;
    [SerializeField] Color acquiredColor;
    [SerializeField] Color notAcquiredColor;

    public void SetWord(string word, bool isAcquired)
    {
        if (isAcquired)
        {
            wordText.text = $"{word}";
            backgroundImage.color = acquiredColor;
        }
        else
        {
            // 단어 길이만 힌트로 표시
            string hint = "";
            for (int i = 0; i < word.Length; i++)
                hint += "? ";
            wordText.text = hint;
            backgroundImage.color = notAcquiredColor;
        }
    }
}