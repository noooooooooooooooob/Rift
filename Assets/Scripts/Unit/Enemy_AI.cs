using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 적 유닛 AI 컨트롤러
/// 간단한 우선순위 기반 AI: 공격 가능하면 공격, 아니면 이동
/// </summary>
public class Enemy_AI : MonoBehaviour
{
    public Unit unit;

    [Header("AI Settings")]
    public float actionDelay = 0.5f;  // 행동 간 딜레이

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }

    /// <summary>
    /// AI 턴 실행 (코루틴)
    /// </summary>
    public IEnumerator ExecuteTurn()
    {
        Debug.Log($"[Enemy_AI] {unit.unitData.unitName} AI 턴 시작");

        yield return new WaitForSeconds(actionDelay);

        // 1. 공격 가능한 타겟 찾기
        Unit target = FindBestTarget();

        if (target != null && IsInAttackRange(target))
        {
            // 공격 범위 내에 타겟이 있으면 공격
            Debug.Log($"[Enemy_AI] {unit.unitData.unitName} → {target.unitData.unitName} 공격!");
            yield return StartCoroutine(ExecuteAttack(target));
        }
        else
        {
            // 공격 범위 밖이면 이동
            if (target != null)
            {
                Debug.Log($"[Enemy_AI] {unit.unitData.unitName} → {target.unitData.unitName} 방향으로 이동");
                yield return StartCoroutine(ExecuteMove(target));
            }
            else
            {
                Debug.Log($"[Enemy_AI] {unit.unitData.unitName} 타겟 없음, 대기");
            }
        }

        yield return new WaitForSeconds(actionDelay);
        Debug.Log($"[Enemy_AI] {unit.unitData.unitName} AI 턴 종료");
    }

    /// <summary>
    /// 최적의 공격 타겟 선택
    /// 우선순위: 가장 HP가 낮은 유닛
    /// </summary>
    private Unit FindBestTarget()
    {
        List<Unit> playerUnits = Battle_Manager.instance.playerUnits
            .Where(u => u != null && !u.isDead)
            .ToList();

        if (playerUnits.Count == 0)
            return null;

        // HP가 가장 낮은 유닛 선택
        Unit weakestTarget = playerUnits
            .OrderBy(u => u.currentHP)
            .FirstOrDefault();

        return weakestTarget;
    }

    /// <summary>
    /// 타겟이 공격 범위 내에 있는지 확인
    /// </summary>
    private bool IsInAttackRange(Unit target)
    {
        if (unit.currentTile == null || target.currentTile == null)
            return false;

        // 기본 공격 범위 (인접 타일)
        int attackRange = 1;
        if (unit.attackAction != null && unit.attackAction.attackSkillData != null)
        {
            
        }

        int distance = Method.GetManhattanDistance(
            unit.currentTile.GridPosition,
            target.currentTile.GridPosition
        );

        return distance <= attackRange;
    }

    /// <summary>
    /// 공격 실행
    /// </summary>
    private IEnumerator ExecuteAttack(Unit target)
    {
        if (unit.attackAction == null)
        {
            Debug.LogWarning($"[Enemy_AI] {unit.unitData.unitName} attackAction이 없습니다!");
            yield break;
        }

        // 공격 애니메이션 재생
        if (unit.unitAnimation != null)
        {
            // unit.unitAnimation.PlayAttackAnimation();
        }

        yield return new WaitForSeconds(0.3f);

        // 데미지 계산 및 적용
        int damage = Method.CalculateDamage(unit, target);

        // 명중 판정
        if (Method.IsHit(unit, target))
        {
            target.TakeDamage(damage);
            Debug.Log($"[Enemy_AI] {target.unitData.unitName}에게 {damage} 데미지!");
        }
        else
        {
            Debug.Log($"[Enemy_AI] {target.unitData.unitName} 회피!");
        }

        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// 타겟 방향으로 이동
    /// </summary>
    private IEnumerator ExecuteMove(Unit target)
    {
        if (unit.moveAction == null)
        {
            Debug.LogWarning($"[Enemy_AI] {unit.unitData.unitName} moveAction이 없습니다!");
            yield break;
        }

        // 이동 가능한 타일 목록
        List<Tile> movableTiles = Method.GetMovableTiles(unit, unit.stats.AGI);

        if (movableTiles.Count == 0)
        {
            Debug.Log($"[Enemy_AI] {unit.unitData.unitName} 이동 가능한 타일 없음");
            yield break;
        }

        // 타겟에게 가장 가까운 타일 선택
        Tile bestTile = movableTiles
            .OrderBy(tile => Method.GetManhattanDistance(tile.GridPosition, target.currentTile.GridPosition))
            .FirstOrDefault();

        if (bestTile != null)
        {
            unit.moveAction.MoveToTile(bestTile);

            // 이동 애니메이션 대기
            if (unit.unitAnimation != null)
            {
                yield return new WaitUntil(() => !unit.unitAnimation.isMoving);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
