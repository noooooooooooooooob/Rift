using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 턴 게이지 UI 컨트롤러
/// 턴 순서 아이콘을 슬라이더 타임라인 위에 표시
/// SPD 기반으로 실시간 게이지 위치를 시각화
/// 게이지 0(오른쪽) → 100(왼쪽)으로 이동
/// </summary>
public class Turn_Gauge : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform gaugeBar;        // 게이지 바 영역
    [SerializeField] private Transform iconContainer;       // 아이콘 부모 Transform
    [SerializeField] private GameObject defaultIconPrefab;  // 기본 아이콘 프리팹 (UnitData에 없을 경우 사용)

    [Header("Settings")]
    [SerializeField] private Color playerIconColor = Color.blue;   // 플레이어 아이콘 색상
    [SerializeField] private Color enemyIconColor = Color.red;     // 적 아이콘 색상

    // 유닛-아이콘 매핑
    private Dictionary<Unit, TurnIconUI> unitIconMap = new Dictionary<Unit, TurnIconUI>();

    /// <summary>
    /// 유닛 배치 시 턴 게이지 UI에 아이콘 추가
    /// Deployment_Controller.PlaceUnit()에서 호출됨
    /// </summary>
    /// <param name="unit">배치된 유닛</param>
    public void AddUnitIcon(Unit unit)
    {
        if (unit == null || unitIconMap.ContainsKey(unit))
            return;

        // 프리팹 결정: UnitData에 있으면 사용, 없으면 기본 프리팹
        GameObject prefab = unit.unitData?.unitTurnGaugePrefab ?? defaultIconPrefab;

        if (prefab == null)
        {
            Debug.LogWarning($"[Turn_Gauge] {unit.unitData?.unitName ?? "Unknown"} 유닛의 턴 게이지 프리팹이 없습니다.");
            return;
        }

        // 아이콘 인스턴스 생성
        GameObject iconObj = Instantiate(prefab, iconContainer);
        TurnIconUI iconUI = iconObj.GetComponent<TurnIconUI>();

        if (iconUI == null)
        {
            iconUI = iconObj.AddComponent<TurnIconUI>();
        }

        // 아이콘 초기화
        iconUI.Initialize(unit, unit.isPlayerUnit ? playerIconColor : enemyIconColor);

        // 매핑에 추가
        unitIconMap[unit] = iconUI;

        // 초기 위치 업데이트
        UpdateIconPosition(unit, iconUI);

        Debug.Log($"[Turn_Gauge] {unit.unitData?.unitName ?? "Unknown"} 아이콘 추가됨");
    }

    /// <summary>
    /// 전투 시작 시 적 유닛들의 아이콘 추가
    /// Turn_Manager.HandleBattlePhase()에서 호출됨
    /// </summary>
    public void InitializeForBattle()
    {
        // 적 유닛들 아이콘 추가
        if (Battle_Manager.instance != null)
        {
            foreach (Unit enemy in Battle_Manager.instance.enemyUnits)
            {
                if (enemy != null && !enemy.isDead)
                {
                    AddUnitIcon(enemy);
                }
            }
        }

        Debug.Log($"[Turn_Gauge] 전투 초기화 완료: 총 {unitIconMap.Count}개 아이콘");
    }

    /// <summary>
    /// 매 프레임 모든 아이콘 위치 업데이트
    /// </summary>
    private void Update()
    {
        UpdateAllIconPositions();
    }

    /// <summary>
    /// 모든 유닛 아이콘의 위치를 현재 게이지 값에 맞게 업데이트
    /// </summary>
    public void UpdateAllIconPositions()
    {
        // 게이지 순으로 정렬된 리스트 생성 (낮은 게이지 → 높은 게이지, 같으면 플레이어 우선)
        List<KeyValuePair<Unit, TurnIconUI>> sortedList = new List<KeyValuePair<Unit, TurnIconUI>>(unitIconMap);
        sortedList.Sort((a, b) =>
        {
            int gaugeCompare = a.Key.turnGauge.CompareTo(b.Key.turnGauge);
            if (gaugeCompare != 0)
                return gaugeCompare;
            // 게이지가 같으면 플레이어 우선 (플레이어가 뒤쪽 = 앞에 렌더링)
            return a.Key.isPlayerUnit.CompareTo(b.Key.isPlayerUnit);
        });

        // 정렬된 순서대로 sibling index 설정 (게이지 높은 캐릭터가 앞에)
        for (int i = 0; i < sortedList.Count; i++)
        {
            Unit unit = sortedList[i].Key;
            TurnIconUI iconUI = sortedList[i].Value;

            if (unit == null || unit.isDead || iconUI == null)
                continue;

            iconUI.transform.SetSiblingIndex(i);
            UpdateIconPosition(unit, iconUI);
        }
    }

    /// <summary>
    /// 개별 아이콘 위치 업데이트
    /// 게이지 0 → 오른쪽, 게이지 100 → 왼쪽
    /// </summary>
    private void UpdateIconPosition(Unit unit, TurnIconUI iconUI)
    {
        if (gaugeBar == null) return;

        float barWidth = gaugeBar.rect.width;
        float normalizedGauge = Mathf.Clamp01(unit.turnGauge / 100f);

        iconUI.UpdatePosition(normalizedGauge, barWidth);
    }

    /// <summary>
    /// 유닛 사망 시 아이콘 제거
    /// Turn_Manager.OnUnitDeath()에서 호출됨
    /// </summary>
    /// <param name="unit">사망한 유닛</param>
    public void RemoveIcon(Unit unit)
    {
        if (unit == null || !unitIconMap.ContainsKey(unit))
            return;

        TurnIconUI iconUI = unitIconMap[unit];
        if (iconUI != null)
        {
            Destroy(iconUI.gameObject);
        }

        unitIconMap.Remove(unit);
        Debug.Log($"[Turn_Gauge] {unit.unitData?.unitName ?? "Unknown"} 아이콘 제거됨");
    }

    /// <summary>
    /// 모든 아이콘 제거 (전투 종료 시)
    /// </summary>
    public void ClearAllIcons()
    {
        foreach (var kvp in unitIconMap)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value.gameObject);
            }
        }
        unitIconMap.Clear();
    }
}
