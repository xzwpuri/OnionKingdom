using UnityEngine;
using Unity.Cinemachine;

public class CombatCameraController : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] CinemachineCamera explorationCam;
    [SerializeField] CinemachineCamera combatCam;
    [SerializeField] CameraFollowProxy followProxy;

    [Header("Priority")]
    [SerializeField] int explorationPriority = 10;
    [SerializeField] int combatActivePriority = 15;

    [Header("Ortho Size")]
    [Tooltip("보스전 기본 줌 (탐색 카메라 OrthoSize보다 약간 크게)")]
    [SerializeField] float baseCombatOrthoSize = 5f;
    [Tooltip("플레이어가 높이 올라갔을 때 최대 줌아웃")]
    [SerializeField] float maxCombatOrthoSize = 8f;
    [Tooltip("플레이어 위/아래로 남길 여백 (world units)")]
    [SerializeField] float playerVisibilityMargin = 1.5f;
    [SerializeField] float orthoSizeLerpSpeed = 3f;

    CinemachineConfiner2D _confiner;
    Transform _player;
    bool _inCombat;
    float _currentOrthoSize;

    void Awake()
    {
        _confiner = combatCam.GetComponent<CinemachineConfiner2D>();
        _player = GameObject.FindWithTag("Player")?.transform;
        _currentOrthoSize = baseCombatOrthoSize;

        combatCam.Priority = explorationPriority - 5;
    }

    void OnEnable()
    {
        BossCombatEvents.OnCombatStart += EnterCombatMode;
        BossCombatEvents.OnCombatEnd += ExitCombatMode;
    }

    void OnDisable()
    {
        BossCombatEvents.OnCombatStart -= EnterCombatMode;
        BossCombatEvents.OnCombatEnd -= ExitCombatMode;
    }

    void EnterCombatMode(Collider2D bounds)
    {
        _inCombat = true;

        if (_confiner != null)
        {
            // CM3 Confiner2D는 PolygonCollider2D / CompositeCollider2D만 지원.
            // BossArena cameraBounds에 PolygonCollider2D를 사용할 것.
            _confiner.BoundingShape2D = bounds;
            _confiner.enabled = true;
            _confiner.InvalidateBoundingShapeCache();
        }

        // 카메라 Y를 아레나 기하학적 중심이 아닌 플레이어의 현재 Y(지면)로 고정.
        // 이렇게 해야 점프할수록 distFromProxyY가 증가해 OrthoSize가 커진다.
        float arenaY = _player != null ? _player.position.y
                       : (bounds != null ? bounds.bounds.center.y : 0f);

        followProxy.Activate(_player, arenaY);

        _currentOrthoSize = baseCombatOrthoSize;
        var lens = combatCam.Lens;
        lens.OrthographicSize = _currentOrthoSize;
        combatCam.Lens = lens;

        // 우선순위 올리면 CinemachineBrain이 DefaultBlend 설정에 따라 자동 블렌딩
        combatCam.Priority = combatActivePriority;
        explorationCam.Priority = explorationPriority;
    }

    void ExitCombatMode()
    {
        _inCombat = false;
        followProxy.Deactivate();

        combatCam.Priority = explorationPriority - 5;

        if (_confiner != null)
            _confiner.enabled = false;
    }

    void LateUpdate()
    {
        if (!_inCombat || _player == null) return;

        // 프록시 Y와 플레이어 Y의 차이만큼 줌아웃해서 플레이어가 항상 화면 안에 보이도록
        float distFromProxyY = Mathf.Abs(_player.position.y - followProxy.transform.position.y);
        float targetSize = Mathf.Clamp(
            Mathf.Max(baseCombatOrthoSize, distFromProxyY + playerVisibilityMargin),
            baseCombatOrthoSize,
            maxCombatOrthoSize
        );

        _currentOrthoSize = Mathf.Lerp(_currentOrthoSize, targetSize, Time.deltaTime * orthoSizeLerpSpeed);

        var lens = combatCam.Lens;
        lens.OrthographicSize = _currentOrthoSize;
        combatCam.Lens = lens;
    }
}
