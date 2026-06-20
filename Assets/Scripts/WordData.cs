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
    Place,
    Eat,
    Hit,
    Shoot
}

public enum DebuffType
{
    None,
    Burn,
    Explosive,
    Slow,
    Stun
}

[CreateAssetMenu(fileName = "WordData", menuName = "Word/WordData")]
public class WordData : ScriptableObject
{
    [Header("Common")]
    public string word;
    public WordType type;

    [Header("Verb (동사일 때)")]
    public VerbType verbType;
    public bool canBeUsedAlone;

    [Header("Noun (명사일 때)")]
    public Sprite worldSprite;
    public float damageModifier;
    public float sizeModifier;

    [Header("Adjective (형용사일 때)")]
    public GameObject particleEffectPrefab;
    public Color outlineColor = Color.white;
    public float adjectiveDamageBonus;
    public DebuffType debuffType;
    public float debuffValue;
    public float debuffDuration;
}