using UnityEngine;
using System.Collections.Generic;

public static class WeaponBuilder
{
    public static WeaponData TryBuild(List<WordData> orderedWords)
    {
        if (orderedWords.Count == 0)
        {
            Debug.Log("문법 오류: 단어가 없음");
            return null;
        }

        if (orderedWords[0].type != WordType.Verb)
        {
            Debug.Log("문법 오류: 첫 단어가 동사가 아님");
            return null;
        }

        WordData verbWord = orderedWords[0];
        VerbType verb = verbWord.verbType;

        List<NounEntry> nounEntries = new List<NounEntry>();
        List<WordData> currentAdjectives = new List<WordData>();

        for (int i = 1; i < orderedWords.Count; i++)
        {
            WordData word = orderedWords[i];

            if (word.type == WordType.Verb)
            {
                Debug.Log("문법 오류: 동사가 맨 앞이 아닌 위치에 있음");
                return null;
            }
            else if (word.type == WordType.Adjective)
            {
                currentAdjectives.Add(word);
            }
            else if (word.type == WordType.Noun)
            {
                NounEntry entry = new NounEntry
                {
                    noun = word,
                    appliedAdjectives = new List<WordData>(currentAdjectives)
                };
                nounEntries.Add(entry);
            }
        }

        // 동사별 검증
        if (verb == VerbType.Hit)
        {
            if (nounEntries.Count > 1)
            {
                Debug.Log("문법 오류: Hit은 명사 1개까지만 가능");
                return null;
            }
            if (nounEntries.Count == 0 && !verbWord.canBeUsedAlone)
            {
                Debug.Log("문법 오류: 이 동사는 단독 사용 불가");
                return null;
            }
        }
        else if (verb == VerbType.Place)
        {
            if (nounEntries.Count != 1)
            {
                Debug.Log("문법 오류: Place는 명사 1개 필수");
                return null;
            }
        }
        else
        {
            if (nounEntries.Count == 0)
            {
                Debug.Log("문법 오류: 명사가 최소 1개 필요");
                return null;
            }
        }

        int wordCount = orderedWords.Count;
        int uses = wordCount switch
        {
            1 => 10,
            2 => 7,
            3 => 5,
            4 => 3,
            _ => 1
        };

        return new WeaponData
        {
            verb = verb,
            nounEntries = nounEntries,
            usesRemaining = uses
        };
    }
}