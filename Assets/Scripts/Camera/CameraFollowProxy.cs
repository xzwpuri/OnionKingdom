using UnityEngine;

/// <summary>
/// 전투 카메라의 Follow 타겟. 플레이어 X만 추적, Y는 아레나 중심에 고정.
/// </summary>
public class CameraFollowProxy : MonoBehaviour
{
    Transform _player;
    float _fixedY;
    bool _active;

    public void Activate(Transform player, float fixedY)
    {
        _player = player;
        _fixedY = fixedY;
        _active = true;

        if (_player != null)
            transform.position = new Vector3(_player.position.x, fixedY, 0f);
    }

    public void Deactivate() => _active = false;

    void LateUpdate()
    {
        if (!_active || _player == null) return;
        transform.position = new Vector3(_player.position.x, _fixedY, 0f);
    }
}
