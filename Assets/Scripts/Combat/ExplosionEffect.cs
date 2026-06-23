using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float duration = 0.5f;

    void Start()
    {
        if (animator != null)
            animator.Play(0);
        Destroy(gameObject, duration);
    }
}
