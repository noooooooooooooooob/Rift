using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

/// <summary>
/// 적 유닛 AI 컨트롤러
/// 간단한 우선순위 기반 AI: 공격 가능하면 공격, 아니면 이동
/// </summary>
public class Enemy_AI : MonoBehaviour
{
    public Unit unit;
    [Header("AI Settings")]
    public float actionDelay = 0.5f;  // 행동 간 딜레이
    [Header("Action")]
    public bool canMove = true;
    public bool canAttack = true;
    public bool canUseSkill1 = false;
    public bool canUseSkill2 = false;
    public bool canGuard = false;
    public bool canUltimate = false;
    [Header("Action Weights")]
    private float weightSum = 0;
    public float attackWeight = 70;
    public float moveWeight = 10;
    public float skill1Weight = 10;
    public float skill2Weight = 10;
    public float guardWeight = 5;
    
    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
        weightSum = attackWeight + moveWeight + skill1Weight + skill2Weight + guardWeight;
    }

    /// <summary>
    /// AI 턴 실행 (코루틴)
    /// </summary>
    public IEnumerator ExecuteTurn()
    {
        Debug.Log($"[Enemy_AI] {unit.unitData.unitName} AI 턴 시작");

        yield return new WaitForSeconds(actionDelay);

        Unit target = FindBestTarget();
        while (true)
        {
            int ran = Random.Range(0, (int)weightSum);
            if(ran < attackWeight && canAttack)
            {
                if(unit.attackAction.attackSkillData.cost > Battle_Manager.instance.enemyActivePoint)   continue;
                // 공격
                if (target != null)
                {
                    yield return ExecuteAttack(target);
                    break;
                }
            }
            else if(ran < attackWeight + moveWeight && canMove)
            {
                // 이동
                if (target != null)
                {
                    yield return ExecuteMove(target);
                    break;
                }
            }
            else if(ran < attackWeight + moveWeight + skill1Weight && canUseSkill1)
            {
                if(unit.skill1Action.skill1Data.cost > Battle_Manager.instance.enemyActivePoint)   continue;
                // 스킬1 사용
                if (target != null)
                {
                    yield return ExecuteSkill1(target);
                    break;
                }
            }
            else if(ran < attackWeight + moveWeight + skill1Weight + skill2Weight && canUseSkill2)
            {
                if(unit.skill2Action.skill2Data.cost > Battle_Manager.instance.enemyActivePoint)   continue;
                // 스킬2 사용
                if (target != null)
                {
                    yield return ExecuteSkill2(target);
                    break;
                }
            }
            else if(ran < attackWeight + moveWeight + skill1Weight + skill2Weight + guardWeight && canGuard)
            {
                // 가드
                yield return ExecuteGuard();
                break;
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

    private IEnumerator ExecuteSkill1(Unit target)
    {
        if (unit.skill1Action == null)
        {
            Debug.LogWarning($"[Enemy_AI] {unit.unitData.unitName} skill1Action이 없습니다!");
            yield break;
        }

        // 스킬1 사용 로직 (임시)
        Debug.Log($"[Enemy_AI] {unit.unitData.unitName}가 스킬1을 사용합니다!");

        yield return new WaitForSeconds(0.5f);
    }
    private IEnumerator ExecuteSkill2(Unit target)
    {
        if (unit.skill2Action == null)
        {
            Debug.LogWarning($"[Enemy_AI] {unit.unitData.unitName} skill2Action이 없습니다!");
            yield break;
        }

        // 스킬2 사용 로직 (임시)
        Debug.Log($"[Enemy_AI] {unit.unitData.unitName}가 스킬2를 사용합니다!");

        yield return new WaitForSeconds(0.5f);
    }
    private IEnumerator ExecuteGuard()
    {
        if (unit.guardAction == null)
        {
            Debug.LogWarning($"[Enemy_AI] {unit.unitData.unitName} guardAction이 없습니다!");
            yield break;
        }

        // 가드 액션 실행
        // unit.guardAction.Guard();

        Debug.Log($"[Enemy_AI] {unit.unitData.unitName}가 가드합니다!");

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

        int randomIdx = Random.Range(0, movableTiles.Count);
        Tile bestTile = movableTiles[randomIdx];

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
