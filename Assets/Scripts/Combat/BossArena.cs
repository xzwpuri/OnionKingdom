using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossArena : MonoBehaviour
{
    [SerializeField] BossBase boss;
    [SerializeField] GameObject[] walls;

    [Header("Camera")]
    [Tooltip("카메라 범위 제한용 PolygonCollider2D (아레나 경계와 같은 모양)")]
    [SerializeField] Collider2D cameraBounds;

    bool activated = false;

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (boss != null)
            boss.OnDefeated += OnBossDefeated;

        SetWallsPassable(true);
    }

    void OnDestroy()
    {
        if (boss != null)
            boss.OnDefeated -= OnBossDefeated;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        SetWallsPassable(false);
        boss?.Activate();
        BossCombatEvents.RaiseCombatStart(cameraBounds);
    }

    void OnBossDefeated()
    {
        SetWallsPassable(true);
        BossCombatEvents.RaiseCombatEnd();
    }

    void SetWallsPassable(bool passable)
    {
        foreach (var wall in walls)
        {
            if (wall == null) continue;
            var col = wall.GetComponent<Collider2D>();
            if (col != null) col.isTrigger = passable;
        }
    }
}
