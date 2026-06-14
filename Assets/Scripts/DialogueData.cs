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
    public string text;
    public int nextPageIndex;
    public DialogueChoice[] choices;
    public BoggleData boggleData;
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
    public int nextPageIndex;
}

[System.Serializable]
public class BoggleData
{
    public string[] boardRows;
    public WordData[] answers;

    public char[,] GetBoard()
    {
        char[,] board = new char[4, 4];
        for (int col = 0; col < 4; col++)
            for (int row = 0; row < 4; row++)
                board[col, row] = boardRows[col][row];
        return board;
    }
}