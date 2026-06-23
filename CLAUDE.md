# CLAUDE.md — OnionKingdom

## 프로젝트 개요
Boggle 방식 단어 획득 × Wuppo 스타일 2D 보스전 액션 게임.
단어(동사+형용사+명사) 조합으로 무기를 생성해 전투. Unity 6.3 LTS (URP, 2D).

**입력**: Legacy Input Manager | **UI**: TextMeshPro + uGUI | **VCS**: GitHub

---

## 디렉토리 구조

```
Assets/
├── Animation/          # HitAnimation.controller, Swing.anim
├── Prefab/             # 모든 게임 프리팹
├── Scripts/
│   ├── Boggle/         # 보글 단어 획득 시스템
│   ├── Boss/           # 보스별 패턴 스크립트
│   ├── Combat/         # 체력/보스 공통 로직
│   ├── Debug/          # 에디터 전용 헬퍼
│   ├── Dialogue/       # NPC 대화 시스템
│   ├── Hotbar/         # TAB 핫바 UI
│   ├── Player/         # 플레이어 이동/넉백
│   ├── Weapon/         # 무기 시스템 전체
│   └── Word/           # 단어 데이터 및 덱 관리
└── Pixel Art/          # VFX 에셋 (서드파티)
```

---

## 핵심 시스템 및 스크립트

### Word / Deck 시스템
| 파일 | 역할 |
|------|------|
| `Scripts/Word/WordData.cs` | ScriptableObject. `WordType`(Verb/Noun/Adjective), `VerbType`(None/Hit/Throw/Shoot/Place/Eat), `DebuffType` 정의 |
| `Scripts/Word/DeckManager.cs` | 싱글톤. 인벤토리/핫바/쿨타임 3단 관리. `AddWord`, `OpenHotbar`, `CloseHotbar`, `RerollHotbar` |

### 무기 시스템
| 파일 | 역할 |
|------|------|
| `Scripts/Weapon/WeaponData.cs` | `NounEntry`(명사+적용형용사) 리스트와 `VerbType`, `usesRemaining` 보관 |
| `Scripts/Weapon/WeaponBuilder.cs` | `TryBuild(List<WordData>)` → 어순 검증 후 `WeaponData` 반환. 사용횟수: 1단어=10회, 2=7, 3=5, 4=3, 5+=1 |
| `Scripts/Weapon/WeaponLibrary.cs` | 정적 클래스. `Hit()`, `Throw()` 구현. 새 동사는 여기에 추가 |
| `Scripts/Weapon/WeaponManager.cs` | 싱글톤. 좌클릭 감지 → `UseWeapon()` → `WeaponLibrary`로 분기. `throwProjectilePrefab`, `hitWeaponPrefab` SerializeField 보유 |
| `Scripts/Weapon/HitWeaponObject.cs` | Hit 동사 투사체 MonoBehaviour. `Setup()` 후 애니메이션+콜라이더 활성화 |
| `Scripts/Weapon/ThrowProjectile.cs` | Throw 동사 투사체. `Setup()`으로 초기화. `Update()`에서 중력 적용 포물선 이동 |
| `Scripts/Weapon/EquippedWeaponDisplay.cs` | 현재 무기/남은 횟수 HUD |
| `Scripts/Weapon/HitNounRelay.cs` | Hit 명사 오브젝트 충돌 → `HitWeaponObject.HandleTrigger()` 전달 |

### 핫바 UI
| 파일 | 역할 |
|------|------|
| `Scripts/Hotbar/HotbarController.cs` | TAB 누름 → `OpenHotbar()`, 뗌 → `CloseHotbar()` + `WeaponBuilder.TryBuild()` 호출 |
| `Scripts/Hotbar/CombineSlotDropZone.cs` | 조합 슬롯 드랍존 |
| `Scripts/Hotbar/DraggableCard.cs` | 카드 드래그앤드롭 |
| `Scripts/Hotbar/HotbarDropZone.cs` | 핫바 영역 드랍존 |
| `Scripts/Hotbar/WordCardView.cs` | 개별 단어 카드 UI |

### 전투/보스
| 파일 | 역할 |
|------|------|
| `Scripts/Combat/Health.cs` | 체력 컴포넌트. `TakeDamage(int, Vector2, ignoreInvincible)`, `OnDeath` 이벤트 |
| `Scripts/Combat/PlayerHealth.cs` | Player 전용 Health 래퍼 |
| `Scripts/Combat/BossBase.cs` | 보스 공통 베이스. 패턴 타이머, 접촉 데미지, DangerZone 관리 |
| `Scripts/Combat/DangerZoneIndicator.cs` | 위험 구역 경고 표시 |
| `Scripts/Boss/BossCarrot.cs` | BossBase 상속. 당근 보스 패턴 |
| `Scripts/Boss/BossMaple.cs` | BossBase 상속. 단풍 보스 패턴 |
| `Scripts/Boss/FallingLeaf.cs` | 단풍잎 낙하 오브젝트 |
| `Scripts/Boss/RootObject.cs` | 뿌리 돌출 오브젝트 |

### 플레이어
| 파일 | 역할 |
|------|------|
| `Scripts/Player/PlayerController.cs` | 이동(Horizontal), 점프(Space, 2단), 중력 스케일, 넉백 처리 |
| `Scripts/Player/PlayerKnockback.cs` | 넉백 로직 분리 |

### Boggle / 대화
| 파일 | 역할 |
|------|------|
| `Scripts/Boggle/BoggleDirector.cs` | 보글 창 진행 총괄 |
| `Scripts/Boggle/BoggleCelllView.cs` | 개별 셀 뷰 |
| `Scripts/Boggle/BoggleLine.cs` | 선택 경로 선 렌더링 |
| `Scripts/Boggle/WordEntryView.cs` | 단어 발견 목록 |
| `Scripts/Dialogue/DialogueData.cs` | 대화 데이터 ScriptableObject |
| `Scripts/Dialogue/DialogueBubble.cs` | 말풍선 UI |
| `Scripts/Dialogue/NPCController.cs` | NPC 상호작용 |
| `Scripts/Dialogue/ChoiceButton.cs` | 선택지 버튼 |

---

## 주요 프리팹

| 프리팹 | 용도 |
|--------|------|
| `Prefab/ThrowProjectile.prefab` | `ThrowProjectile.cs` 부착. Throw 투사체 |
| `Prefab/HitWeapon.prefab` | `HitWeaponObject.cs` 부착. Hit 무기 이펙트 |
| `Prefab/DangerZone.prefab` | `DangerZoneIndicator.cs` 부착 |
| `Prefab/FallingLeaf.prefab` | `FallingLeaf.cs` 부착. BossMaple 패턴 |
| `Prefab/Root.prefab` / `SurgeRoot.prefab` | `RootObject.cs` 부착. BossCarrot 패턴 |
| `Prefab/BoggleView.prefab` | 보글 UI 전체 |
| `Prefab/WordCard.prefab` | 핫바 카드 1장 |

---

## 동사 구현 패턴 (새 동사 추가 체크리스트)

1. **`WeaponLibrary.cs`**: 정적 메서드 추가 (`public static void Shoot(...)` 또는 코루틴)
2. **새 ProjectileScript**: `MonoBehaviour` 작성 후 `Scripts/Weapon/` 에 저장
3. **새 Prefab**: `Assets/Prefab/` 에 프리팹 생성, 스크립트 부착
4. **`WeaponManager.cs`**: `[SerializeField]` 필드 추가 + `switch` 케이스 추가
5. **`WeaponBuilder.cs`**: 필요 시 동사별 검증 로직 추가

---

## 데모 구현 우선순위 (GAME_DESIGN.md §11 기준)

1. **Shoot 동사 구현** ← 현재 작업
2. Adjective 디버프 구현 (Burn, Explosive)
3. 플레이어/보스 애니메이션 상태 머신
4. 보스 처치 후 흐름
5. Place, Eat — 데모 이후 보류 (`VerbType` enum은 이미 선언됨)

---

## 코딩 컨벤션
- 언어: C# (Unity 6 MonoBehaviour 패턴)
- 새 동사 투사체는 `ThrowProjectile` 구조 참고 (`Setup()` + `Update()` + `OnTriggerEnter2D()`)
- 데미지 기본값 `10f` + `noun.damageModifier` + adjective 보너스 합산
- 보스 충돌 데미지: `ignoreInvincible: true` 필수
- 싱글톤: `DontDestroyOnLoad` 패턴 (`WeaponManager`, `DeckManager`)
- 애니메이션: 현재 placeholder. `HitAnimation.controller` + `Swing.anim`만 존재
