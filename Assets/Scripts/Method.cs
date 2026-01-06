using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 전투 계산 공식 유틸리티 클래스
/// 명중/회피, 방어력, 피해 계산, 생명력 흡수, 반격 등의 전투 메커니즘 구현
/// 모든 메서드는 static으로 전역적으로 사용 가능
/// </summary>
public static class Method
{
    private const float DEF_K = 100f; // 방어력 감소 상수 (DEF / (DEF + K) 공식)

    // ========== 1. HIT(회피) 판정 ==========
    /// <summary>
    /// 공격이 명중했는지 판정
    /// 방어자의 회피율(EV)에 따라 확률적으로 회피 가능
    /// </summary>
    /// <returns>true: 명중, false: 회피</returns>
    public static bool IsHit(Unit attacker, Unit defender)
    {
        // 현재는 명중 100%, 회피만 적용
        float evasion = defender.stats.EV; // 0~1

        // Random.value < EV 이면 회피
        if (Random.value < evasion)
            return false;

        return true;
    }

    // ========== 2. 유효 방어력 계산(관통 적용) ==========
    /// <summary>
    /// 방어구 관통(AP)을 적용한 유효 방어력 계산
    /// 유효 방어력 = 원래 방어력 - 방어구 관통 (최소 0)
    /// </summary>
    public static float GetEffectiveDefense(Unit attacker, Unit defender)
    {
        float def = defender.stats.DEF;
        float ap = attacker.stats.AP;

        // 최소 0
        return Mathf.Max(0f, def - ap);
    }

    // ========== 3. 방어력 기반 피해 감소율 ==========
    /// <summary>
    /// 방어력을 피해 감소율로 변환
    /// 공식: DEF / (DEF + K) - 방어력이 높을수록 감소율 증가, 최대 1에 수렴
    /// </summary>
    /// <returns>0~1 범위의 피해 감소율</returns>
    public static float GetDefenseReduction(float effectiveDef)
    {
        // DEF / (DEF + K)
        return effectiveDef / (effectiveDef + DEF_K);
    }

    // ========== 4. DR(Damage Reduction) 결합 ==========
    /// <summary>
    /// 방어력 기반 감소율과 DR(Damage Reduction) 스탯을 결합
    /// 두 감소율을 곱셈 방식으로 조합: 1 - (1 - DEF감소) * (1 - DR)
    /// </summary>
    /// <returns>0~0.99 범위의 총 피해 감소율</returns>
    public static float GetTotalReduction(Unit attacker, Unit defender)
    {
        float effectiveDef = GetEffectiveDefense(attacker, defender);
        float defReduction = GetDefenseReduction(effectiveDef); // 0~1

        float dr = defender.stats.DR; // 0~0.99

        // (1 - DEF) * (1 - DR) 조합
        float total = 1f - (1f - defReduction) * (1f - dr);

        return Mathf.Clamp(total, 0f, 0.99f);
    }

    // ========== 5. 최종 데미지 계산 ==========
    /// <summary>
    /// 최종 피해량 계산
    /// 1. 기본 피해 = ATK * multiplier + flatBonus
    /// 2. 총 피해 감소율 적용
    /// 3. 최소 1 피해 보장
    /// </summary>
    /// <param name="multiplier">공격력 배율 (스킬 계수 등)</param>
    /// <param name="flatBonus">고정 추가 피해</param>
    /// <returns>최종 피해량 (정수)</returns>
    public static int CalculateDamage(Unit attacker, Unit defender, float multiplier = 1f, float flatBonus = 0f)
    {
        float atk = attacker.stats.ATK;

        // 기본 데미지
        float rawDamage = atk * multiplier + flatBonus;

        // 총 피해 감소율
        float reduction = GetTotalReduction(attacker, defender);

        float final = rawDamage * (1f - reduction);

        // 최소 1
        return Mathf.Max(1, Mathf.RoundToInt(final));
    }

    // ========== 6. 생명력 흡수 ==========
    /// <summary>
    /// 생명력 흡수량 계산
    /// 최종 피해의 LS(Life Steal) 비율만큼 회복
    /// </summary>
    /// <param name="finalDamage">최종 피해량</param>
    /// <returns>회복량 (정수)</returns>
    public static int CalculateLifeSteal(Unit attacker, int finalDamage)
    {
        float ls = attacker.stats.LS;  // 0~1
        if (ls <= 0f) return 0;

        return Mathf.RoundToInt(finalDamage * ls);
    }

    // ========== 7. 반격 판정 ==========
    /// <summary>
    /// 반격 발동 여부 판정
    /// 방어자의 CA(Counter Attack) 확률에 따라 확률적으로 발동
    /// </summary>
    /// <returns>true: 반격 발동, false: 반격 없음</returns>
    public static bool TryCounter(Unit defender)
    {
        float ca = defender.stats.CA;  // 0~1
        return Random.value < ca;
    }

    // ========== 8. 레이어 재귀적 설정 ==========
    /// <summary>
    /// GameObject와 모든 자식 오브젝트의 레이어를 재귀적으로 설정
    /// 컷신 렌더링을 위한 Focus 레이어 설정 등에 사용
    /// </summary>
    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    // ========== 9. 맨해튼 거리 계산 ==========
    /// <summary>
    /// 두 타일 간의 맨해튼 거리 계산
    /// </summary>
    public static int GetManhattanDistance(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }

    /// <summary>
    /// 유닛과 타일 간의 거리 계산
    /// </summary>
    public static int GetDistance(Unit unit, Tile tile)
    {
        if (unit.currentTile == null || tile == null) return int.MaxValue;
        return GetManhattanDistance(unit.currentTile.GridPosition, tile.GridPosition);
    }

    // ========== 10. 이동 가능 타일 계산 (BFS) ==========
    /// <summary>
    /// 유닛이 이동 가능한 모든 타일 리스트 반환
    /// BFS를 사용하여 실제 도달 가능한 타일만 반환 (장애물 우회 경로 고려)
    /// </summary>
    /// <param name="unit">이동할 유닛</param>
    /// <param name="moveRange">이동 범위 (기본 2)</param>
    /// <returns>이동 가능한 타일 리스트</returns>
    public static List<Tile> GetTilesSoloTarget(Unit unit, int moveRange, bool ignoreOccupants = false)
    {
        List<Tile> movableTiles = new List<Tile>();

        if (unit.currentTile == null || Battle_Manager.instance == null)
            return movableTiles;

        Vector2Int startPos = unit.currentTile.GridPosition;
        Queue<(Tile tile, int distance)> queue = new Queue<(Tile, int)>();
        Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();

        // 시작 타일 추가
        queue.Enqueue((unit.currentTile, 0));
        visited[startPos] = 0;

        // 4방향 벡터 (상하좌우)
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // 위
            new Vector2Int(0, -1),  // 아래
            new Vector2Int(-1, 0),  // 왼쪽
            new Vector2Int(1, 0)    // 오른쪽
        };

        while (queue.Count > 0)
        {
            var (currentTile, currentDistance) = queue.Dequeue();
            Vector2Int currentPos = currentTile.GridPosition;

            // 이동 범위 초과 시 스킵
            if (currentDistance >= moveRange)
                continue;

            // 4방향 탐색
            foreach (Vector2Int dir in directions)
            {
                Vector2Int nextPos = currentPos + dir;

                // 그리드 범위 체크
                if (nextPos.x < 0 || nextPos.x >= Battle_Manager.GridWidth ||
                    nextPos.y < 0 || nextPos.y >= Battle_Manager.GridHeight)
                    continue;

                // 이미 더 짧은 거리로 방문했으면 스킵
                if (visited.ContainsKey(nextPos) && visited[nextPos] <= currentDistance + 1)
                    continue;

                Tile nextTile = Battle_Manager.instance.tiles[nextPos.x, nextPos.y];

                if (nextTile == null)
                    continue;

                // 점유된 타일은 통과 불가
                if (nextTile.IsOccupied && !ignoreOccupants)
                    continue;

                // 방문 처리 및 큐 추가
                visited[nextPos] = currentDistance + 1;
                queue.Enqueue((nextTile, currentDistance + 1));
                movableTiles.Add(nextTile);
            }
        }

        return movableTiles;
    }
}
