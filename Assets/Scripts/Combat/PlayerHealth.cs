using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] hpStageSprites; // HP 단계별 스프라이트 (낮은 인덱스 = 정상, 높은 인덱스 = 위급)

    private void Awake()
    {
        health.OnHPChanged += UpdateSprite;
        health.OnDeath += HandleDeath;
    }

    private void UpdateSprite(int currentHP, int maxHP)
    {
        if (hpStageSprites == null || hpStageSprites.Length == 0) return;

        // HP 비율에 따라 스프라이트 단계 계산
        float ratio = (float)currentHP / maxHP;
        int stageIndex = Mathf.Clamp(
            Mathf.FloorToInt((1f - ratio) * hpStageSprites.Length),
            0,
            hpStageSprites.Length - 1
        );

        spriteRenderer.sprite = hpStageSprites[stageIndex];
    }

    private void HandleDeath()
    {
        Debug.Log("플레이어 사망 처리");
        // TODO: 사망 처리 (리스폰, 게임오버 등)
    }
}