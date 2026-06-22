using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] HitWeaponObject hitWeaponPrefab;
    [SerializeField] ThrowProjectile throwProjectilePrefab;
    [SerializeField] EquippedWeaponDisplay weaponDisplay;

    public static WeaponManager Instance { get; private set; }

    WeaponData currentWeapon;
    Coroutine activeRoutine;
    Transform player;

    public WeaponData CurrentWeapon => currentWeapon;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (weaponDisplay != null)
            weaponDisplay.gameObject.SetActive(false);

        CachePlayer();
    }

    private void CachePlayer()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null)
        {
            CachePlayer(); // 씬 전환 등으로 플레이어가 늦게 생성될 경우 대비
            if (player == null) return;
        }

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
        if (player == null) return;

        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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

        StartCoroutine(ReshowDisplayAfterDelay(0.3f));

        Debug.Log($"무기 사용 | 남은 횟수: {currentWeapon.usesRemaining}");
    }

    private IEnumerator ReshowDisplayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentWeapon != null && currentWeapon.usesRemaining > 0 && weaponDisplay != null)
        {
            weaponDisplay.gameObject.SetActive(true);
            weaponDisplay.UpdateUses(currentWeapon.usesRemaining);
        }
    }
}