# Rift 전술 RPG - 균형잡힌 개발 계획

## 프로젝트 현황

### ✅ 완성된 시스템
- **배치 시스템** (90%): Deployment_Controller - 드래그 앤 드롭 완전 구현
- **타일 시스템** (100%): Tile, TileMapCreator - 10x5 그리드 완성
- **카메라 시스템** (85%): CameraManager, MainCameraController - Cinemachine 완성
- **입력 시스템** (70%): ClickController, IClickable - New Input System 사용

### ⚠️ 부분 구현
- **Turn_Manager**: Deployment Phase만 구현, Battle/Result 주석 처리
- **Battle_Manager**: 기본 구조만 존재, playerUnits/enemyUnits 초기화 없음
- **Unit.cs**: CurrentTile, unitData만 있음 → **stats, currentHP 등 전투 필드 전무**

### ❌ 미구현
- **UI 시스템**: 완전히 없음 (CharacterInfoPanel 참조만 주석)
- **전투 로직**: Method.cs 모든 함수 주석 처리
- **TurnGuage_Manager**: 빈 플레이스홀더
- **스킬/AI 시스템**: 없음

### 🔴 치명적 문제
1. **UnitStat**: `[System.Serializable]` 없음 → Inspector에서 편집 불가
2. **Unit.cs**: stats 필드 없음 → Method.cs 전투 공식 사용 불가 (컴파일 에러)
3. **유닛 데이터**: Serian 1개만 존재, baseStat 미설정

---

## 개발 전략

### 사용자 선택사항
- ✅ **균형잡힌 병행 개발**: 전투, UI, 데이터 동시 진행
- ✅ **Method.cs 주석 코드 활성화**: 기존 설계 사용
- ✅ **게이지 기반 턴 시스템**: TurnGuage_Manager 구현
- ✅ **Unit.cs에 stats 필드 추가**: 권장 방식

### 마일스톤 구조
각 마일스톤은 **전투(Combat) + UI + 데이터(Data)**를 균형있게 포함하여 단계별 테스트 가능.

---

## Phase 1: 핵심 인프라 구축 (1~2주)

### 목표
전투 시스템의 기반 데이터 구조 완성 및 기본 전투 공식 활성화

### 1.1 데이터 구조 수정

#### 📝 `Assets/Scripts/Unit/Unit Stat.cs` - 직렬화 추가
```csharp
[System.Serializable]  // ← 추가 필수!
public class UnitStat
{
    // 기존 필드에 Range 어트리뷰트 추가
    [Range(0f, 1f)] public float LS;
    [Range(0f, 1f)] public float RST;
    [Range(0f, 1f)] public float EV;
    [Range(0f, 1f)] public float CA;
    [Range(0f, 0.99f)] public float DR;

    // Clone() 메서드 추가
    public UnitStat Clone()
    {
        return new UnitStat
        {
            HP = this.HP,
            UP = this.UP,
            ATK = this.ATK,
            DEF = this.DEF,
            SPD = this.SPD,
            AGI = this.AGI,
            LS = this.LS,
            RST = this.RST,
            EV = this.EV,
            CA = this.CA,
            DR = this.DR,
            AP = this.AP
        };
    }
}
```

**중요**: `[System.Serializable]` 없으면 ScriptableObject에서 baseStat 편집 불가!

#### 📝 `Assets/Scripts/Unit/Unit.cs` - 전투 필드 추가
```csharp
public class Unit : MonoBehaviour
{
    // 기존
    public Tile CurrentTile;
    public UnitData unitData;

    // ✨ 새로 추가
    [Header("Runtime Stats")]
    public UnitStat stats;          // 현재 스탯 (버프 적용된 값)
    public int currentHP;
    public int maxHP;
    public float turnGauge;         // 0~100

    [Header("Battle Info")]
    public bool isPlayerUnit;
    public bool isDead = false;

    // 초기화 메서드 (Battle_Manager에서 호출)
    public void Initialize(bool isPlayer)
    {
        isPlayerUnit = isPlayer;

        if (unitData != null && unitData.baseStat != null)
        {
            stats = unitData.baseStat.Clone();  // 깊은 복사!
            maxHP = stats.HP;
            currentHP = maxHP;
        }

        turnGauge = 0f;
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        if (currentHP <= 0)
        {
            isDead = true;
            Debug.Log($"{gameObject.name} has died!");
        }
    }

    public void Heal(int amount)
    {
        if (!isDead)
            currentHP = Mathf.Min(maxHP, currentHP + amount);
    }
}
```

**핵심**: stats는 런타임 복사본, unitData.baseStat은 원본 데이터.

#### 📝 `Assets/Scripts/Method.cs` - 주석 제거 (전체 파일)
- 모든 주석 `//` 제거하여 코드 활성화
- `IsHit()`, `CalculateDamage()`, `CalculateLifeSteal()`, `TryCounter()` 등
- `SetLayerRecursively()`는 이미 활성화되어 있음

**변경사항**: 주석만 제거, 로직은 그대로 사용. Unit.stats 필드와 연동됨.

### 1.2 기본 UI 구조

#### 📝 새 파일: `Assets/Scripts/UI/UIManager.cs`
```csharp
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Panels")]
    public GameObject deploymentPanel;
    public GameObject battlePanel;
    public GameObject resultPanel;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void ShowDeploymentUI()
    {
        deploymentPanel?.SetActive(true);
        battlePanel?.SetActive(false);
        resultPanel?.SetActive(false);
    }

    public void ShowBattleUI()
    {
        deploymentPanel?.SetActive(false);
        battlePanel?.SetActive(true);
        resultPanel?.SetActive(false);
    }

    public void ShowResultUI()
    {
        deploymentPanel?.SetActive(false);
        battlePanel?.SetActive(false);
        resultPanel?.SetActive(true);
    }
}
```

**설계**: Phase별 UI 전환만 담당. 각 패널은 추후 단계에서 구현.

### 1.3 Unity Editor 작업
1. Serian.asset 열기
2. baseStat 필드가 보이는지 확인 (UnitStat 직렬화 후)
3. 임시 값 입력:
   - HP: 100, ATK: 20, DEF: 10, SPD: 15, AGI: 12
   - LS: 0.1, EV: 0.2, CA: 0.15, DR: 0.1, AP: 5
4. 저장 및 재실행하여 값 유지 확인

### 테스트
- Serain.asset Inspector에서 baseStat 편집 가능한지
- 임시 테스트 스크립트로 데미지 계산 검증:
```csharp
void Start() {
    testUnit.Initialize(true);
    int damage = Method.CalculateDamage(attacker, defender);
    Debug.Log($"Damage: {damage}");
}
```

---

## Phase 2: 턴 게이지 시스템 (1~2주)

### 목표
SPD 기반 동적 턴 순서 시스템 및 턴 순서 UI 표시

### 2.1 TurnGuage_Manager 완전 구현

#### 📝 `Assets/Scripts/Battle/TurnGuage_Manager.cs` - 전체 재작성
```csharp
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnGuage_Manager : MonoBehaviour
{
    public static TurnGuage_Manager instance;

    [Header("Balance")]
    public float gaugeIncreaseRate = 1f;
    public float maxGauge = 100f;

    private List<Unit> allUnits = new List<Unit>();

    void Awake() { instance = this; }

    // 전투 시작 시 초기화
    public void InitializeTurnSystem(List<Unit> playerUnits, List<Unit> enemyUnits)
    {
        allUnits.Clear();
        allUnits.AddRange(playerUnits);
        allUnits.AddRange(enemyUnits);

        foreach (var unit in allUnits)
            unit.turnGauge = 0f;
    }

    // 매 프레임 게이지 증가
    public void UpdateTurnGauges(float deltaTime)
    {
        foreach (var unit in allUnits)
        {
            if (unit.isDead) continue;
            unit.turnGauge += unit.stats.SPD * gaugeIncreaseRate * deltaTime;
        }
    }

    // 턴 받을 유닛 있는지
    public bool HasUnitReadyForTurn()
    {
    return allUnits.Any(u => !u.isDead && u.turnGauge >= maxGauge);
    }

    // 다음 턴 유닛 가져오기
    public Unit GetNextTurnUnit()
    {
        var readyUnits = allUnits
            .Where(u => !u.isDead && u.turnGauge >= maxGauge)
            .OrderByDescending(u => u.turnGauge)
            .ToList();

        if (readyUnits.Count == 0) return null;

        Unit nextUnit = readyUnits[0];
        nextUnit.turnGauge -= maxGauge;  // 게이지 소모
        return nextUnit;
    }

    // UI용: 예상 턴 순서 계산
    public List<Unit> GetUpcomingTurnOrder(int count = 5)
    {
        // 시뮬레이션을 통해 다음 N개 턴 예측
        Dictionary<Unit, float> simulated = new Dictionary<Unit, float>();
        foreach (var u in allUnits)
            if (!u.isDead) simulated[u] = u.turnGauge;

        List<Unit> predicted = new List<Unit>();

        for (int i = 0; i < count; i++)
        {
            var ready = simulated
                .Where(kvp => kvp.Value >= maxGauge)
                .OrderByDescending(kvp => kvp.Value)
                .FirstOrDefault();

            if (ready.Key != null)
            {
                predicted.Add(ready.Key);
                simulated[ready.Key] -= maxGauge;
            }
            else
            {
                // 시간 진행 시뮬레이션
                float minTime = float.MaxValue;
                foreach (var kvp in simulated)
                {
                    float remaining = maxGauge - kvp.Value;
                    float timeNeeded = remaining / (kvp.Key.stats.SPD * gaugeIncreaseRate);
                    if (timeNeeded < minTime) minTime = timeNeeded;
                }

                foreach (var unit in simulated.Keys.ToList())
                    simulated[unit] += unit.stats.SPD * gaugeIncreaseRate * minTime;

                i--; // 다시 확인
            }
        }

        return predicted;
    }

    public void OnUnitDeath(Unit unit)
    {
        allUnits.Remove(unit);
    }
}
```

### 2.2 Turn_Manager Battle Phase 구현

#### 📝 `Assets/Scripts/Battle/Turn_Manager.cs` - HandleBattlePhase() 추가
```csharp
private IEnumerator HandleBattlePhase()
{
    Debug.Log("Battle Phase Started");
    UIManager.instance?.ShowBattleUI();

    // 턴 시스템 초기화
    turnGaugeManager.InitializeTurnSystem(
        Battle_Manager.instance.playerUnits,
        Battle_Manager.instance.enemyUnits
    );

    // 전투 루프
    while (!IsBattleOver())
    {
        // 게이지 증가
        turnGaugeManager.UpdateTurnGauges(Time.deltaTime);

        // 턴 받을 유닛 확인
        if (turnGaugeManager.HasUnitReadyForTurn())
        {
            currentTurnUnit = turnGaugeManager.GetNextTurnUnit();

            if (currentTurnUnit != null)
            {
                yield return StartCoroutine(ExecuteUnitTurn(currentTurnUnit));
            }
        }

        yield return null;
    }

    Debug.Log("Battle Phase Ended");
    currentPhase = BattlePhase.Result;
}

private IEnumerator ExecuteUnitTurn(Unit unit)
{
    Debug.Log($"=== {unit.gameObject.name}'s Turn ===");

    if (unit.isPlayerUnit)
        yield return StartCoroutine(HandlePlayerTurn(unit));
    else
        yield return StartCoroutine(HandleAITurn(unit));
}

// 플레이어/AI 턴 처리 (Phase 3에서 구현)
private IEnumerator HandlePlayerTurn(Unit unit)
{
    yield return new WaitForSeconds(1f); // 임시
}

private IEnumerator HandleAITurn(Unit unit)
{
    yield return new WaitForSeconds(0.5f); // 임시
}

private bool IsBattleOver()
{
    bool allPlayersDead = Battle_Manager.instance.playerUnits.TrueForAll(u => u.isDead);
    bool allEnemiesDead = Battle_Manager.instance.enemyUnits.TrueForAll(u => u.isDead);
    return allPlayersDead || allEnemiesDead;
}
```

### 2.3 턴 순서 UI

#### 📝 새 파일: `Assets/Scripts/UI/TurnOrderPanel.cs`
```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderPanel : MonoBehaviour
{
    public Transform turnIconContainer;
    public GameObject turnIconPrefab;

    private List<GameObject> turnIcons = new List<GameObject>();

    void Update()
    {
        if (TurnGuage_Manager.instance != null)
        {
            var upcoming = TurnGuage_Manager.instance.GetUpcomingTurnOrder(5);
            UpdateTurnOrder(upcoming);
        }
    }

    void UpdateTurnOrder(List<Unit> units)
    {
        // 기존 아이콘 제거
        foreach (var icon in turnIcons)
            Destroy(icon);
        turnIcons.Clear();

        // 새 아이콘 생성
        foreach (var unit in units)
        {
            GameObject icon = Instantiate(turnIconPrefab, turnIconContainer);
            Image img = icon.GetComponent<Image>();
            if (img && unit.unitData.unitPortrait)
                img.sprite = unit.unitData.unitPortrait;
            turnIcons.Add(icon);
        }
    }
}
```

**Unity Editor**:
- Canvas 생성 → TurnOrderPanel 추가
- Horizontal Layout Group 설정
- turnIconPrefab: Image 프리팹 생성

### 테스트
- 2개 유닛 배치 (SPD 10, SPD 20)
- Battle Phase 진입
- Console에서 턴 순서 확인 (SPD 20이 2배 빠르게 턴 획득)
- UI에 턴 순서 표시 확인

---

## 나머지 Phase는 문서를 참고하세요

전체 개발 계획은 7개 Phase로 구성되어 있으며, 각 Phase는 1~3주 소요됩니다.

- **Phase 3**: 기본 전투 시스템 (2~3주)
- **Phase 4**: UI 시스템 확장 (1~2주)
- **Phase 5**: 이동 시스템 (1~2주)
- **Phase 6**: 적 배치 및 AI (1~2주)
- **Phase 7**: Result Phase (1주)

**예상 총 개발 기간**: 13~14주 (약 3~4개월)

---

## 치명적 파일 (반드시 먼저 수정)

### 1순위 (컴파일 에러 해결)
1. **`Assets/Scripts/Unit/Unit Stat.cs`** - `[System.Serializable]` 추가, Clone() 구현
2. **`Assets/Scripts/Unit/Unit.cs`** - stats, currentHP, maxHP, turnGauge 필드 추가
3. **`Assets/Scripts/Method.cs`** - 모든 주석 제거

### 2순위 (핵심 시스템)
4. **`Assets/Scripts/Battle/TurnGuage_Manager.cs`** - 완전히 새로 작성
5. **`Assets/Scripts/Battle/Turn_Manager.cs`** - HandleBattlePhase() 구현
