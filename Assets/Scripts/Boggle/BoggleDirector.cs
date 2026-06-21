using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class BoggleDirector : MonoBehaviour
{
    int maxCol = 4;
    int maxRow = 4;

    [SerializeField] BoggleCellView cellPrefab;
    [SerializeField] RectTransform boggleGridRect;

    [Header("Word List UI")]
    [SerializeField] WordEntryView wordEntryPrefab;
    [SerializeField] RectTransform wordListContent;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] TextMeshProUGUI currentWordText;

    [Header("Line")]
    [SerializeField] BoggleLine linePrefab;
    [SerializeField] RectTransform lineContainer;

    [Header("Colors")]
    [SerializeField] Color normalColor;
    [SerializeField] Color selectColor;
    [SerializeField] Color correctColor;
    [SerializeField] Color wrongColor;

    char[,] data;
    WordData[] answerList;

    HashSet<string> acquiredWords = new HashSet<string>();
    List<WordEntryView> wordEntryViews = new List<WordEntryView>();
    List<BoggleLine> activeLines = new List<BoggleLine>();

    string ans = "";
    List<BoggleCellView> path = new List<BoggleCellView>();
    bool isDrag = false;

    public Action<WordData> OnWordAcquired;
    public Action OnAllWordsAcquired;

    public void Init(char[,] boardData, WordData[] answers)
    {
        foreach (Transform child in boggleGridRect)
            Destroy(child.gameObject);

        foreach (Transform child in wordListContent)
            Destroy(child.gameObject);

        ClearLines();

        data = boardData;
        answerList = answers;
        acquiredWords.Clear();
        wordEntryViews.Clear();
        path.Clear();
        ans = "";
        isDrag = false;
        currentWordText.text = "";

        // DeckManager 기반으로 획득 여부 체크
        if (DeckManager.Instance != null)
        {
            foreach (var word in answerList)
            {
                if (DeckManager.Instance.HasWord(word))
                    acquiredWords.Add(word.word.ToUpper());
            }
        }

        SetBoard(data);
        SetWordListUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
            LeavePath();
    }

    private void SetBoard(char[,] data)
    {
        for (int col = 0; col < maxCol; col++)
        {
            for (int row = 0; row < maxRow; row++)
            {
                BoggleCellView tmp = Instantiate(cellPrefab, boggleGridRect);
                tmp.SetCell(col, row, data[col, row]);
                tmp.CellPointerEnter += AddPath;
                tmp.CellPointerDown += EnterPath;
                tmp.CellPointerDown += AddPath;
            }
        }
    }

    private void SetWordListUI()
    {
        foreach (WordData word in answerList)
        {
            bool isAcquired = acquiredWords.Contains(word.word.ToUpper());
            WordEntryView entry = Instantiate(wordEntryPrefab, wordListContent);
            entry.SetWord(word.word, isAcquired);
            wordEntryViews.Add(entry);
        }
        UpdateCountText();
    }

    private void UpdateCountText()
    {
        countText.text = $"{acquiredWords.Count} / {answerList.Length} Acquired";
    }

    private void RefreshWordListUI()
    {
        for (int i = 0; i < answerList.Length; i++)
        {
            bool isAcquired = acquiredWords.Contains(answerList[i].word.ToUpper());
            wordEntryViews[i].SetWord(answerList[i].word, isAcquired);
        }
        UpdateCountText();
    }

    private void EnterPath(BoggleCellView cell)
    {
        isDrag = true;
    }

    private void AddPath(BoggleCellView cell)
    {
        if (!isDrag) return;

        int existingIndex = path.IndexOf(cell);

        if (existingIndex >= 0)
        {
            for (int i = path.Count - 1; i > existingIndex; i--)
            {
                path[i].UnSelected(normalColor);
                ans = ans.Substring(0, ans.Length - 1);
                path.RemoveAt(i);

                if (activeLines.Count > 0)
                {
                    Destroy(activeLines[activeLines.Count - 1].gameObject);
                    activeLines.RemoveAt(activeLines.Count - 1);
                }
            }
        }
        else
        {
            if (path.Count > 0)
            {
                BoggleCellView peek = path[path.Count - 1];
                int dx = Math.Abs(peek.Col - cell.Col);
                int dy = Math.Abs(peek.Row - cell.Row);
                if (dx > 1 || dy > 1) return;

                DrawLine(peek, cell, selectColor);
            }

            path.Add(cell);
            cell.SetSelected(selectColor);
            ans += data[cell.Col, cell.Row];
        }

        currentWordText.text = ans;
    }

    private void DrawLine(BoggleCellView from, BoggleCellView to, Color color)
    {
        BoggleLine line = Instantiate(linePrefab, lineContainer);

        Camera cam = Camera.main;

        Vector2 fromScreen = RectTransformUtility.WorldToScreenPoint(cam, from.GetComponent<RectTransform>().position);
        Vector2 toScreen = RectTransformUtility.WorldToScreenPoint(cam, to.GetComponent<RectTransform>().position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineContainer, fromScreen, cam, out Vector2 fromLocal);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineContainer, toScreen, cam, out Vector2 toLocal);

        line.SetLine(fromLocal, toLocal, color);
        activeLines.Add(line);
    }

    private void ClearLines()
    {
        foreach (var line in activeLines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        activeLines.Clear();
    }

    private void UpdateLineColors(Color color)
    {
        foreach (var line in activeLines)
        {
            if (line != null)
                line.GetComponent<Image>().color = color;
        }
    }

    private void LeavePath()
    {
        isDrag = false;

        string upperAns = ans.ToUpper();

        WordData foundWord = answerList.FirstOrDefault(w => w.word.ToUpper() == upperAns);

        if (foundWord != null && !acquiredWords.Contains(upperAns))
        {
            foreach (var cell in path)
                cell.SetColor(correctColor);
            UpdateLineColors(correctColor);

            acquiredWords.Add(upperAns);
            OnWordAcquired?.Invoke(foundWord);

            // DeckManager에 단어 추가
            if (DeckManager.Instance != null)
                DeckManager.Instance.AddWord(foundWord);

            RefreshWordListUI();

            if (acquiredWords.Count == answerList.Length)
            {
                OnAllWordsAcquired?.Invoke();
                Debug.Log("All words acquired!");
            }
        }
        else
        {
            foreach (var cell in path)
                cell.SetColor(wrongColor);
            UpdateLineColors(wrongColor);
        }

        StartCoroutine(ClearPathDelayed());
    }

    private System.Collections.IEnumerator ClearPathDelayed()
    {
        yield return new WaitForSeconds(0.3f);

        foreach (var cell in path)
            cell.UnSelected(normalColor);

        ClearLines();
        path.Clear();
        ans = "";
        currentWordText.text = "";
    }
}