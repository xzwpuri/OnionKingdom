// =====================================================================
// AnimParam — Animator 파라미터/트리거 이름 상수 모음
// =====================================================================
// Unity 에디터에서 Animator Controller를 만들 때 아래 이름을 그대로 사용하면 됩니다.
//
// ── 플레이어 Animator Controller ─────────────────────────────────────
// Parameters:
//   IsMoving  (Bool)    — 수평 이동 중 여부
//   IsGrounded(Bool)    — 착지 여부 (false = 공중)
//   Attack    (Trigger) — 무기 사용 시 1회 발동
//   Hurt      (Trigger) — 피격 시 1회 발동
//   Death     (Trigger) — 사망 시 1회 발동
//
// States & Transitions:
//   Idle    (기본값)
//   Move    ← IsMoving == true  / → IsMoving == false
//   Jump    ← IsGrounded == false (Any State)
//            → Idle/Move : IsGrounded == true
//   Attack  ← Attack trigger (Any State) → Idle (exit time)
//   Hurt    ← Hurt trigger   (Any State) → Idle (exit time)
//   Death   ← Death trigger  (Any State) → (없음, 루프 없음)
//
// ── BossCarrot Animator Controller ──────────────────────────────────
// Parameters:
//   IsMoving      (Bool)    — 점프 이동 중
//   Hurt          (Trigger)
//   Death         (Trigger)
//   PatternHighJump (Trigger) — 높은 점프 패턴 시작
//   PatternDig      (Trigger) — 땅 파기 패턴 시작
//
// States:
//   Idle / Move / PatternHighJump / PatternDig / Hurt / Death
//   패턴 State는 exit time으로 자동 Idle 복귀
//
// ── BossMaple Animator Controller ───────────────────────────────────
// Parameters:
//   Hurt              (Trigger)
//   Death             (Trigger)
//   PatternRootGrowth (Trigger) — 뿌리 과성장 패턴
//   PatternAutumnWind (Trigger) — 가을 바람 패턴
//   PatternRootSurge  (Trigger) — 뿌리 급성장 패턴
//
// States:
//   Idle / PatternRootGrowth / PatternAutumnWind / PatternRootSurge / Hurt / Death
// =====================================================================

public static class AnimParam
{
    // Bool
    public const string IsMoving   = "IsMoving";
    public const string IsGrounded = "IsGrounded";

    // Trigger (공통)
    public const string Attack = "Attack";
    public const string Hurt   = "Hurt";
    public const string Death  = "Death";

    // Trigger (BossCarrot 패턴)
    public const string CarrotHighJump = "PatternHighJump";
    public const string CarrotDig      = "PatternDig";

    // Trigger (BossMaple 패턴)
    public const string MapleRootGrowth  = "PatternRootGrowth";
    public const string MapleAutumnWind  = "PatternAutumnWind";
    public const string MapleRootSurge   = "PatternRootSurge";
}
