using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;

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

    char[,] data;
    string[] answerList;

    HashSet<string> acquiredWords = new HashSet<string>();
    List<WordEntryView> wordEntryViews = new List<WordEntryView>();

    string ans = "";
    List<BoggleCellView> path = new List<BoggleCellView>();
    bool isDrag = false;

    public Action<string> OnWordAcquired;
    public Action OnAllWordsAcquired;

    private void Start()
    {
        char[,] testData =
        {
            {'Q', 'V', 'S', 'M'},
            {'S', 'S', 'K', 'Z'},
            {'E', 'R', 'T', 'O'},
            {'B', 'U', 'N', 'A'}
        };
        string[] testAnswers = { "BURN", "RUN", "NURSE", "STUN" };

        Init(testData, testAnswers);
    }

    public void Init(char[,] boardData, string[] answers)
    {
        foreach (Transform child in boggleGridRect)
            Destroy(child.gameObject);

        foreach (Transform child in wordListContent)
            Destroy(child.gameObject);

        data = boardData;
        answerList = answers;
        acquiredWords.Clear();
        wordEntryViews.Clear();
        path.Clear();
        ans = "";
        isDrag = false;
        currentWordText.text = "";

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
        foreach (string word in answerList)
        {
            WordEntryView entry = Instantiate(wordEntryPrefab, wordListContent);
            entry.SetWord(word, false);
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
            bool isAcquired = acquiredWords.Contains(answerList[i]);
            wordEntryViews[i].SetWord(answerList[i], isAcquired);
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
                path[i].UnSelected();
                ans = ans.Substring(0, ans.Length - 1);
                path.RemoveAt(i);
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
            }

            path.Add(cell);
            cell.SetSelected();
            ans += data[cell.Col, cell.Row];
        }

        // 현재 선택중인 글자 실시간 표시
        currentWordText.text = ans;
    }

    private void LeavePath()
    {
        isDrag = false;

        string upperAns = ans.ToUpper();

        if (answerList.Contains(upperAns) && !acquiredWords.Contains(upperAns))
        {
            acquiredWords.Add(upperAns);
            OnWordAcquired?.Invoke(upperAns);
            RefreshWordListUI();

            if (acquiredWords.Count == answerList.Length)
            {
                OnAllWordsAcquired?.Invoke();
                Debug.Log("All words acquired!");
            }
        }

        foreach (var cell in path)
            cell.UnSelected();

        path.Clear();
        ans = "";
        currentWordText.text = "";
    }
}