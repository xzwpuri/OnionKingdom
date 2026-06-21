using UnityEngine;
using TMPro;

public class EquippedWeaponDisplay : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] TextMeshPro usesText;
    [SerializeField] Sprite defaultHitSprite;

    [Header("Orbit Settings")]
    [SerializeField] float orbitDistance = 1.2f;

    public void Setup(Sprite sprite)
    {
        Sprite finalSprite = sprite != null ? sprite : defaultHitSprite;

        if (finalSprite != null)
        {
            spriteRenderer.sprite = finalSprite;
            spriteRenderer.enabled = true;
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

        Vector2 basePos = (Vector2)player.position + direction * orbitDistance;
        transform.position = basePos;

        bool flip = direction.x < 0;
        if (spriteRenderer != null)
            spriteRenderer.flipX = flip;
    }
}