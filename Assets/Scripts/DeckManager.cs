using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [Header("Hotbar Settings")]
    [SerializeField] int hotbarSize = 5;

    List<WordData> inventory = new List<WordData>();
    List<WordData> hotbar = new List<WordData>();
    List<WordData> cooldown = new List<WordData>();

    // 읽기 전용 프로퍼티
    public IReadOnlyList<WordData> Inventory => inventory;
    public IReadOnlyList<WordData> Hotbar => hotbar;
    public IReadOnlyList<WordData> Cooldown => cooldown;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // 보글에서 단어 획득 시 호출
    public void AddWord(WordData word)
    {
        inventory.Add(word);
        Debug.Log($"단어 획득: {word.word} | 인벤토리: {inventory.Count}개");
    }

    // 단어를 이미 보유중인지 확인 (보글 획득 여부 체크용)
    public bool HasWord(WordData word)
    {
        return inventory.Any(w => w == word)
            || hotbar.Any(w => w == word)
            || cooldown.Any(w => w == word);
    }

    // 핫바 열기
    public List<WordData> OpenHotbar()
    {
        hotbar.Clear();

        if (inventory.Count == 0)
        {
            Debug.Log("인벤토리가 비어있음");
            return hotbar;
        }

        // 동사 먼저 1개 보장
        List<WordData> verbs = inventory.Where(w => w.type == WordType.Verb).ToList();
        List<WordData> nonVerbs = inventory.Where(w => w.type != WordType.Verb).ToList();

        if (verbs.Count > 0)
        {
            WordData verb = verbs[Random.Range(0, verbs.Count)];
            hotbar.Add(verb);
            inventory.Remove(verb);
        }

        // 나머지 슬롯 채우기
        int remaining = hotbarSize - hotbar.Count;
        List<WordData> pool = inventory.OrderBy(w => Random.value).Take(remaining).ToList();
        foreach (var w in pool)
        {
            hotbar.Add(w);
            inventory.Remove(w);
        }

        Debug.Log($"핫바 오픈: {hotbar.Count}개");
        return hotbar;
    }

    // 핫바 닫기
    public void CloseHotbar()
    {
        // 쿨타임 → 인벤토리
        inventory.AddRange(cooldown);
        cooldown.Clear();

        // 핫바 → 쿨타임
        cooldown.AddRange(hotbar);
        hotbar.Clear();

        Debug.Log($"핫바 닫힘 | 인벤토리: {inventory.Count} | 쿨타임: {cooldown.Count}");
    }

    // 핫바 리롤
    public List<WordData> RerollHotbar()
    {
        // 현재 핫바 → 쿨타임
        cooldown.AddRange(hotbar);
        hotbar.Clear();

        // 새 핫바 뽑기
        return OpenHotbar();
    }

    // 조합에 사용된 단어를 핫바에서 제거
    public void RemoveFromHotbar(List<WordData> usedWords)
    {
        foreach (var word in usedWords)
            hotbar.Remove(word);
    }

    // 사용 횟수 계산
    public int GetWeaponUses(int wordCount)
    {
        return wordCount switch
        {
            1 => 10,
            2 => 7,
            3 => 5,
            4 => 3,
            _ => 1  // 5단어 이상
        };
    }
}