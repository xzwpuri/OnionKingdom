using System.Collections.Generic;

public class NounEntry
{
    public WordData noun;
    public List<WordData> appliedAdjectives = new List<WordData>();
}

public class WeaponData
{
    public VerbType verb;
    public List<NounEntry> nounEntries = new List<NounEntry>();
    public int usesRemaining;
}