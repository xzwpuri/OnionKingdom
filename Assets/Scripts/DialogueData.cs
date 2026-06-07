using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialoguePage[] pages;
}

[System.Serializable]
public class DialoguePage
{
    public PageType pageType;
    public string text;                  // 텍스트 페이지일 때
    public int nextPageIndex;            // 텍스트 페이지 클릭 시 이동할 페이지 (-1이면 종료)
    public DialogueChoice[] choices;     // 선택지 페이지일 때
    public BoggleData boggleData;        // 보글 페이지일 때
}

public enum PageType
{
    Text,
    Choice,
    Boggle
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextPageIndex;            // 이 선택지 고를 시 이동할 페이지 (-1이면 종료)
}

[System.Serializable]
public class BoggleData
{
    public string[] boardRows;
    public string[] answers;

    public char[,] GetBoard()
    {
        char[,] board = new char[4, 4];
        for (int col = 0; col < 4; col++)
            for (int row = 0; row < 4; row++)
                board[col, row] = boardRows[col][row];
        return board;
    }
}