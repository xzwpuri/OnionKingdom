using UnityEngine;
using TMPro;

public class EquippedWeaponDisplay : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] TextMeshPro usesText;
    [SerializeField] Sprite defaultHitSprite; // Hit 단독 사용 시 기본 스프라이트

    [Header("Float Settings")]
    [SerializeField] float orbitDistance = 1.2f;
    [SerializeField] float bobAmount = 0.1f;
    [SerializeField] float bobSpeed = 2f;

    float bobTimer = 0f;

    public void Setup(Sprite sprite)
    {
        Sprite finalSprite = sprite != null ? sprite : defaultHitSprite;

        if (finalSprite != null)
        {
            spriteRenderer.sprite = finalSprite;
            spriteRenderer.enabled = true; // gameObject.SetActive 대신 enabled 사용
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }

    public void UpdateUses(int uses)
    {
        if (usesText != null)
            usesText.text = uses.ToString();
    }

    private void Update()
    {
        if (player == null) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - (Vector2)player.position).normalized;

        bobTimer += Time.deltaTime * bobSpeed;
        float bobOffset = Mathf.Sin(bobTimer) * bobAmount;

        Vector2 basePos = (Vector2)player.position + direction * orbitDistance;
        transform.position = basePos + Vector2.up * bobOffset;

        bool flip = direction.x < 0;
        if (spriteRenderer != null)
            spriteRenderer.flipX = flip;
    }
}