using System;
using UnityEngine;

public static class BossCombatEvents
{
    public static event Action<Collider2D> OnCombatStart;
    public static event Action OnCombatEnd;

    public static void RaiseCombatStart(Collider2D confineBounds)
        => OnCombatStart?.Invoke(confineBounds);

    public static void RaiseCombatEnd()
        => OnCombatEnd?.Invoke();
}
