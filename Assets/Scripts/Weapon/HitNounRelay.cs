using UnityEngine;

public class HitNounRelay : MonoBehaviour
{
    HitWeaponObject parent;

    private void Awake()
    {
        parent = GetComponentInParent<HitWeaponObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        parent?.HandleTrigger(other);
    }
}