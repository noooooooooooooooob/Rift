using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 턴 게이지 시스템 관리자
/// SPD 스탯 기반으로 실시간 턴 게이지를 증가시키고 턴 순서를 결정
/// 에픽세븐 스타일의 동적 턴 시스템 구현
/// </summary>
public class TurnGuage_Manager : MonoBehaviour
{
    public static TurnGuage_Manager instance;  // 싱글톤 인스턴스

    [Header("Balance Settings")]
    [Tooltip("게이지 증가 속도 조절 (높을수록 전투가 빠름)")]
    public float gaugeIncreaseRate = 1f;  // 밸런스 조절용 상수
    public const float MAX_GAUGE = 100f;  // 턴 획득에 필요한 최대 게이지

    private List<Unit> allUnits = new List<Unit>();  // 전투에 참여 중인 모든 유닛

    /// <summary>
    /// 싱글톤 초기화
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 전투 시작 시 턴 시스템 초기화
    /// 모든 유닛의 게이지를 0으로 설정
    /// </summary>
    public void InitializeTurnSystem(List<Unit> playerUnits, List<Unit> enemyUnits)
    {
        allUnits.Clear();
        allUnits.AddRange(playerUnits);
        allUnits.AddRange(enemyUnits);

        // 모든 유닛의 턴 게이지를 0으로 초기화
        foreach (var unit in allUnits)
        {
            if (unit != null)
                unit.turnGauge = 0f;
        }

        Debug.Log($"[TurnGauge] 턴 시스템 초기화 완료: {allUnits.Count}명의 유닛");
    }

    /// <summary>
    /// 매 프레임 모든 살아있는 유닛의 게이지를 SPD에 비례하여 증가
    /// </summary>
    public void UpdateTurnGauges(float deltaTime)
    {
        foreach (var unit in allUnits)
        {
            if (unit == null || unit.isDead) continue;

            // 게이지 증가: SPD * 증가율 * 델타타임
            unit.turnGauge += unit.stats.SPD * gaugeIncreaseRate * deltaTime;
        }
    }

    /// <summary>
    /// 턴을 받을 준비가 된 유닛이 있는지 확인
    /// </summary>
    /// <returns>게이지가 100 이상인 유닛이 있으면 true</returns>
    public bool HasUnitReadyForTurn()
    {
        return allUnits.Any(u => u != null && !u.isDead && u.turnGauge >= MAX_GAUGE);
    }

    /// <summary>
    /// 다음 턴을 받을 유닛을 반환 (게이지 소모는 Turn_Manager에서 처리)
    /// 동점일 경우 게이지가 가장 높은 유닛 우선
    /// </summary>
    /// <returns>턴을 받을 유닛 (없으면 null)</returns>
    public Unit GetNextTurnUnit()
    {
        // 게이지가 100 이상인 유닛들 중 가장 높은 유닛 찾기
        var readyUnits = allUnits
            .Where(u => u != null && !u.isDead && u.turnGauge >= MAX_GAUGE)
            .OrderByDescending(u => u.turnGauge)  // 타이브레이커: 게이지가 더 높은 유닛 우선
            .ToList();

        if (readyUnits.Count == 0)
            return null;

        Unit nextUnit = readyUnits[0];

        return nextUnit;
    }

    /// <summary>
    /// 앞으로 예상되는 턴 순서를 시뮬레이션 (UI 표시용)
    /// 실제 게이지는 변경하지 않음
    /// </summary>
    /// <param name="count">예측할 턴 수</param>
    /// <returns>예상 턴 순서 유닛 리스트</returns>
    public List<Unit> GetUpcomingTurnOrder(int count = 7)
    {
        // 시뮬레이션용 게이지 복사본 생성
        Dictionary<Unit, float> simGauges = new Dictionary<Unit, float>();
        foreach (var u in allUnits)
        {
            if (u != null && !u.isDead)
                simGauges[u] = u.turnGauge;
        }

        List<Unit> predicted = new List<Unit>();

        for (int i = 0; i < count; i++)
        {
            // 게이지가 100 이상인 유닛 찾기
            var ready = simGauges
                .Where(kvp => kvp.Value >= MAX_GAUGE)
                .OrderByDescending(kvp => kvp.Value)
                .FirstOrDefault();

            if (ready.Key != null)
            {
                // 턴 순서에 추가하고 게이지 100 소모
                predicted.Add(ready.Key);
                simGauges[ready.Key] -= MAX_GAUGE;
            }
            else
            {
                // 아무도 준비 안 됐으면 시간을 진행시켜 다음 유닛이 준비될 때까지 시뮬레이션
                if (simGauges.Count == 0) break;

                // 다음 유닛이 100에 도달하는 데 필요한 최소 시간 계산
                float minTimeToReady = float.MaxValue;
                foreach (var kvp in simGauges)
                {
                    if (kvp.Key.stats.SPD <= 0) continue;

                    float remaining = MAX_GAUGE - kvp.Value;
                    float timeNeeded = remaining / (kvp.Key.stats.SPD * gaugeIncreaseRate);

                    if (timeNeeded < minTimeToReady)
                        minTimeToReady = timeNeeded;
                }

                if (minTimeToReady == float.MaxValue) break;

                // 시뮬레이션 시간 진행
                foreach (var unit in simGauges.Keys.ToList())
                {
                    simGauges[unit] += unit.stats.SPD * gaugeIncreaseRate * minTimeToReady;
                }

                i--;  // 이번 반복에서 유닛을 추가하지 않았으므로 다시 시도
            }
        }

        return predicted;
    }

    /// <summary>
    /// 유닛 사망 시 호출되어 리스트에서 제거
    /// </summary>
    public void OnUnitDeath(Unit unit)
    {
        if (allUnits.Contains(unit))
        {
            allUnits.Remove(unit);
            Debug.Log($"[TurnGauge] {unit.unitData.unitName} 사망으로 턴 시스템에서 제거");
        }
    }
}
