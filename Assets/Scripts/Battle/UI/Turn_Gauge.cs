using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 턴 게이지 UI 컨트롤러
/// 턴 순서 아이콘을 표시하고 업데이트
/// </summary>
public class Turn_Gauge : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject turnIconPrefab;

    [Header("Containers")]
    public Transform turnIconsContainer;

    [Header("Settings")]
    [Tooltip("표시할 턴 순서 개수 (기본 7개)")]
    public int maxDisplayedTurns = 7;

    [Tooltip("턴 순서 업데이트 간격 (초)")]
    public float updateInterval = 0.1f;

    // 캐시
    private List<TurnIconUI> turnIcons = new List<TurnIconUI>();
    private List<Unit> previousTurnOrder = new List<Unit>();
    private float updateTimer = 0f;

    /// <summary>
    /// 초기화
    /// </summary>
    public void Initialize()
    {
        ClearAllIcons();
        updateTimer = 0f;
    }

    /// <summary>
    /// 턴 게이지 업데이트 시작
    /// </summary>
    public void StartUpdating()
    {
        enabled = true;
    }

    void Update()
    {
        updateTimer += Time.deltaTime;

        // 지정된 간격마다 턴 순서 업데이트
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateTurnOrderDisplay();
        }
    }

    /// <summary>
    /// 턴 순서 표시 업데이트
    /// GetUpcomingTurnOrder를 호출하여 다음 턴 순서를 가져와 아이콘으로 표시
    /// </summary>
    void UpdateTurnOrderDisplay()
    {
        if (TurnGuage_Manager.instance == null)
        {
            Debug.LogWarning("Turn_Gauge: TurnGuage_Manager.instance is null");
            return;
        }

        // 다음 턴 순서 예측
        List<Unit> upcomingTurns = TurnGuage_Manager.instance.GetUpcomingTurnOrder(maxDisplayedTurns);

        // 턴 순서가 변경되었는지 확인
        if (!AreTurnOrdersSame(upcomingTurns, previousTurnOrder))
        {
            RefreshTurnIcons(upcomingTurns);
            previousTurnOrder = new List<Unit>(upcomingTurns);
        }
    }

    /// <summary>
    /// 턴 아이콘 재생성
    /// 기존 아이콘을 제거하고 새로운 순서로 아이콘을 생성
    /// </summary>
    /// <param name="upcomingTurns">표시할 턴 순서 유닛 리스트</param>
    void RefreshTurnIcons(List<Unit> upcomingTurns)
    {
        // 기존 아이콘 제거
        ClearAllIcons();

        if (turnIconPrefab == null || turnIconsContainer == null)
        {
            Debug.LogWarning("Turn_Gauge: turnIconPrefab or turnIconsContainer is null");
            return;
        }

        // 새 아이콘 생성
        for (int i = 0; i < upcomingTurns.Count; i++)
        {
            Unit unit = upcomingTurns[i];
            if (unit == null || unit.unitData == null) continue;

            // 아이콘 생성
            GameObject iconObj = Instantiate(turnIconPrefab, turnIconsContainer);
            TurnIconUI icon = iconObj.GetComponent<TurnIconUI>();

            if (icon != null)
            {
                // 아이콘 설정
                icon.Setup(unit);

                // 첫 번째 아이콘은 다음 턴으로 강조
                icon.SetIsNextTurn(i == 0);

                // 순차적으로 페이드 인 애니메이션
                icon.AnimateIn(i * 0.05f);

                turnIcons.Add(icon);
            }
        }
    }

    /// <summary>
    /// 모든 턴 아이콘 제거
    /// </summary>
    void ClearAllIcons()
    {
        // 기존 아이콘 제거
        foreach (var icon in turnIcons)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }
        turnIcons.Clear();
    }

    /// <summary>
    /// 두 턴 순서 리스트가 같은지 비교
    /// </summary>
    /// <returns>같으면 true, 다르면 false</returns>
    bool AreTurnOrdersSame(List<Unit> a, List<Unit> b)
    {
        if (a.Count != b.Count) return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }

        return true;
    }

    /// <summary>
    /// 유닛 사망 시 호출 (선택 사항)
    /// 턴 순서는 자동으로 갱신되므로 별도 처리 불필요
    /// </summary>
    public void OnUnitDeath(Unit unit)
    {
        // TurnGuage_Manager가 이미 사망 유닛을 제외하므로
        // 다음 UpdateTurnOrderDisplay()에서 자동으로 반영됨
    }
}
