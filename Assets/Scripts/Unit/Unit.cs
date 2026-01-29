using UnityEngine;

/// <summary>
/// 유닛의 핵심 컴포넌트
/// 전투 스탯, HP, 타일 위치, 생사 상태 등을 관리
/// UnitData ScriptableObject로부터 기본 데이터를 가져와 런타임 스탯을 초기화
/// </summary>
public class Unit : MonoBehaviour
{
    // 기존 필드
    public Tile currentTile;  // 현재 위치한 타일
    public UnitData unitData;  // 유닛 기본 데이터 (ScriptableObject)

    // 런타임 스탯 (전투 중 변경 가능)
    [Header("Runtime Stats")]
    public UnitStat stats;          // 현재 스탯 (버프/디버프 적용된 값)
    public int currentHP;            // 현재 체력
    public int maxHP;                // 최대 체력
    public int currentUP;            // 현재 궁극기 포인트
    public int guard;              // 현재 방어 상태 (방어 스킬 사용 시 증가)
    public float turnGauge;          // 턴 게이지 (0~100, SPD/AGI 기반으로 증가)

    // 전투 정보
    [Header("Battle Info")]
    public bool isPlayerUnit;        // 플레이어 유닛인지 적 유닛인지
    public bool isDead = false;      // 사망 여부

    [Header("UI")]
    public Action_Menu_UI actionMenuUI; // 액션 메뉴 UI 참조
    public RectTransform uiAnchor;      // 유닛 UI 앵커 위치
    public Transform originalUIParent;  // 원래 UI 부모 (되돌리기용)
    public Unit_Bar hpBar;
    public Unit_Bar upBar;

    [Header("Actions")]
    public Unit_Ultimate ultimateAction; // 궁극기 액션 참조
    public Unit_Attack attackAction;     // 기본 공격 액션 참조
    public Unit_Skill1 skill1Action;       // 스킬 액션 참조
    public Unit_Skill2 skill2Action;       // 스킬 액션 참조
    public Unit_Guard guardAction;       // 방어 액션 참조
    public Unit_Move moveAction;         // 이동 액션 참조

    [Header("References")]
    // 매니저 참조 (전투 시작 시 설정)
    private Turn_Manager turnManager;
    public Unit_Animation unitAnimation; // 유닛 애니메이션 컴포넌트 참조
    public EffectHandler effectHandler; // 이펙트 핸들러 컴포넌트 참조

    /// <summary>
    /// 유닛 초기화 (Battle_Manager 또는 배치 완료 후 호출)
    /// unitData로부터 기본 스탯을 복사하여 독립적인 런타임 스탯 생성
    /// </summary>
    public void Initialize(bool isPlayer, Turn_Manager manager = null)
    {
        isPlayerUnit = isPlayer;
        turnManager = manager;

        if (unitData != null && unitData.baseStat != null)
        {
            // baseStat을 복사하여 독립적인 런타임 스탯 생성
            stats = unitData.baseStat.Clone();
            maxHP = stats.HP;
            currentHP = maxHP;
        }
        else
        {
            Debug.LogError($"Unit {gameObject.name}: unitData or baseStat is null!");
        }

        turnGauge = 0f;
        isDead = false;
    }

    /// <summary>
    /// 공격 처리
    /// </summary>
    public void Attack(Unit defender, int damage)
    {
        defender.TakeDamage(damage);
    }

    /// <summary>
    /// 데미지 받기
    /// HP를 감소시키고 0 이하가 되면 사망 처리
    /// </summary>
    public void TakeDamage(int damage, DamageTextType type = DamageTextType.Normal)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        hpBar.UpdateBar(currentHP, maxHP);

        // 대미지 텍스트 표시
        Vector3 textPos = transform.position + Vector3.up * 1.0f;
        VFX_Manager.Instance?.PlayDamageText(textPos, damage, type);

        if (currentHP <= 0)
        {
            isDead = true;
            OnDeath();
        }
    }

    /// <summary>
    /// 체력 회복
    /// 최대 HP를 초과하지 않도록 제한, 사망 상태에서는 불가
    /// </summary>
    public void Heal(int amount)
    {
        if (isDead) return;

        int actualHeal = Mathf.Min(maxHP - currentHP, amount);
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        hpBar.UpdateBar(currentHP, maxHP);

        // 힐 텍스트 표시 (실제 회복량이 있을 때만)
        if (actualHeal > 0)
        {
            Vector3 textPos = transform.position + Vector3.up * 1.0f;
            VFX_Manager.Instance?.PlayDamageText(textPos, actualHeal, DamageTextType.Heal);
        }
    }

    /// <summary>
    /// 최대 UP를 초과하지 않도록 제한, 사망 상태에서는 불가
    /// </summary>
    public void UpdateUP(int amount)
    {
        if (isDead) return;
        currentUP = Mathf.Min(stats.UP, currentUP + amount);
        upBar.UpdateBar(currentUP, stats.UP);
    }

    /// <summary>
    /// 턴 받아옴
    /// </summary>
    public bool GetTurn()
    {
        return true;
    }

    /// <summary>
    /// 사망 처리
    /// TurnGaugeManager와 Turn_Manager에게 사망을 알리고 승패 체크를 트리거
    /// </summary>
    private void OnDeath()
    {
        Debug.Log($"{gameObject.name} has died!");

        // TurnGaugeManager에게 사망 알림 (턴 시스템에서 제거)
        if (TurnGuage_Manager.instance != null)
            TurnGuage_Manager.instance.OnUnitDeath(this);

        // Turn_Manager에게 사망 알림 (승패 체크 트리거)
        if (turnManager != null)
            turnManager.OnUnitDeath(this);

        
        if (isPlayerUnit)
        {
            // 플레이어 유닛은 비활성화만 (부활 가능성)
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
