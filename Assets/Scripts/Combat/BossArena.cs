using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossArena : MonoBehaviour
{
    [SerializeField] BossBase boss;
    [SerializeField] GameObject[] walls;

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
    }

    void OnBossDefeated()
    {
        SetWallsPassable(true);
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
