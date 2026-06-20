using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] HitWeaponObject hitWeaponPrefab;
    [SerializeField] ThrowProjectile throwProjectilePrefab;
    public static WeaponManager Instance { get; private set; }

    WeaponData currentWeapon;
    Coroutine activeRoutine;

    public WeaponData CurrentWeapon => currentWeapon;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (currentWeapon == null) return;
        if (currentWeapon.usesRemaining <= 0)
        {
            DestroyWeapon();
            return;
        }

        if (Input.GetMouseButtonDown(0))
            UseWeapon();
    }

    public void EquipWeapon(WeaponData weapon)
    {
        DestroyWeapon();
        currentWeapon = weapon;
        Debug.Log($"무기 장착: {weapon.verb} | 사용 {weapon.usesRemaining}회");
    }

    public void DestroyWeapon()
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);
        currentWeapon = null;
    }

    private void UseWeapon()
    {
        if (currentWeapon == null || currentWeapon.usesRemaining <= 0) return;

        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Transform player = GameObject.FindWithTag("Player").transform;

        switch (currentWeapon.verb)
        {
            case VerbType.Hit:
                WeaponLibrary.Hit(this, player, currentWeapon, targetPos, hitWeaponPrefab);
                break;
            case VerbType.Throw:
                WeaponLibrary.Throw(this, player, currentWeapon, targetPos, throwProjectilePrefab);
                break;
        }

        currentWeapon.usesRemaining--;
        Debug.Log($"무기 사용 | 남은 횟수: {currentWeapon.usesRemaining}");
    }
}