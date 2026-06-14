using UnityEngine;

public enum WordType
{
    Verb,
    Noun,
    Adjective,
    Adverb
}

public enum VerbType
{
    None,
    Throw,
    Install,
    Eat,
    Hit,
    Shoot
}

[CreateAssetMenu(fileName = "WordData", menuName = "Word/WordData")]
public class WordData : ScriptableObject
{
    public string word;
    public WordType type;
    public VerbType verbType;       // type이 Verb일 때만 사용
    public bool canBeUsedAlone;     // 1단어 단독 조합 가능 여부 (현재 Hit만 true)
    public Sprite icon;             // 추후 UI용
}