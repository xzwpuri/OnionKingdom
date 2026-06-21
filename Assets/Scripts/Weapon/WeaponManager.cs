using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] HitWeaponObject hitWeaponPrefab;
    [SerializeField] ThrowProjectile throwProjectilePrefab;
    [SerializeField] EquippedWeaponDisplay weaponDisplay; // 씬에 있는 오브젝트 직접 참조

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

    private void Start()
    {
        if (weaponDisplay != null)
            weaponDisplay.gameObject.SetActive(false);
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

        if (weaponDisplay != null)
        {
            weaponDisplay.gameObject.SetActive(true);
            weaponDisplay.Setup(GetDisplaySprite(weapon));
            weaponDisplay.UpdateUses(weapon.usesRemaining);
        }

        Debug.Log($"무기 장착: {weapon.verb} | 사용 {weapon.usesRemaining}회");
    }

    public void DestroyWeapon()
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        if (weaponDisplay != null)
            weaponDisplay.gameObject.SetActive(false);

        currentWeapon = null;
    }

    private Sprite GetDisplaySprite(WeaponData weapon)
    {
        if (weapon.nounEntries.Count > 0)
            return weapon.nounEntries[0].noun.worldSprite;
        return null;
    }

    private void UseWeapon()
    {
        if (currentWeapon == null || currentWeapon.usesRemaining <= 0) return;

        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Transform player = GameObject.FindWithTag("Player").transform;

        // 사용하는 동안 디스플레이 숨김
        if (weaponDisplay != null)
            weaponDisplay.gameObject.SetActive(false);

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

        // 사용 모션 끝난 뒤 다시 보이게
        StartCoroutine(ReshowDisplayAfterDelay(0.3f)); // 사용 모션 길이에 맞게 조절

        Debug.Log($"무기 사용 | 남은 횟수: {currentWeapon.usesRemaining}");
    }

    private System.Collections.IEnumerator ReshowDisplayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentWeapon != null && currentWeapon.usesRemaining > 0 && weaponDisplay != null)
        {
            weaponDisplay.gameObject.SetActive(true);
            weaponDisplay.UpdateUses(currentWeapon.usesRemaining);
        }
    }
}