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
    List<Tile> movableTiles = new List<Tile>();
    List<Tile> attackableTiles = new List<Tile>();
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
    public virtual IEnumerator ExecuteTurn()
    {
        Debug.Log($"[Enemy_AI] {unit.unitData.unitName} AI 턴 시작");

        yield return new WaitForSeconds(actionDelay);

        while (true)
        {
            int ran = Random.Range(0, (int)weightSum);
            if(ran < attackWeight && canAttack)
            {
                if(unit.attackAction.attackSkillData.cost > Battle_Manager.instance.enemyActivePoint) continue;

                // 공격 - 스킬 타겟 타입에 따라 타겟 선택
                Unit target = FindTarget(unit.attackAction.attackSkillData);
                if (target != null)
                {
                    attackableTiles = Method.GetAttackableTiles(unit, unit.attackAction.attackSkillData, target.currentTile);
                    HighlightTiles(attackableTiles, HighlightType.Attackable);
                    yield return new WaitForSeconds(0.3f);
                    yield return unit.attackAction.ExecuteAttack(attackableTiles);
                    ClearHighlights(attackableTiles);
                    break;
                }
            }
            else if(ran < attackWeight + moveWeight && canMove)
            {
                // 이동
                movableTiles = Method.GetMovableTiles(unit, unit.stats.AGI);
                if (movableTiles.Count > 0)
                {
                    int ranidx = Random.Range(0, movableTiles.Count);
                    Tile bestTile = movableTiles[ranidx];
                    unit.moveAction.MoveToTile(bestTile);
                    break;
                }
            }
            else if(ran < attackWeight + moveWeight + skill1Weight && canUseSkill1)
            {
                if(unit.skill1Action.skill1Data.cost > Battle_Manager.instance.enemyActivePoint) continue;

                // 스킬1 - 스킬 타겟 타입에 따라 타겟 선택
                Unit target = FindTarget(unit.skill1Action.skill1Data);
                if (target != null)
                {
                    attackableTiles = Method.GetAttackableTiles(unit, unit.skill1Action.skill1Data, target.currentTile);
                    HighlightTiles(attackableTiles, HighlightType.Attackable);
                    yield return new WaitForSeconds(0.3f);
                    yield return unit.skill1Action.ExecuteSkill1(attackableTiles);
                    ClearHighlights(attackableTiles);
                    break;
                }
            }
            else if(ran < attackWeight + moveWeight + skill1Weight + skill2Weight && canUseSkill2)
            {
                if(unit.skill2Action.skill2Data.cost > Battle_Manager.instance.enemyActivePoint) continue;

                // 스킬2 - 스킬 타겟 타입에 따라 타겟 선택
                Unit target = FindTarget(unit.skill2Action.skill2Data);
                if (target != null)
                {
                    yield return ExecuteSkill2(target);
                    break;
                }
            }
            else if(ran < attackWeight + moveWeight + skill1Weight + skill2Weight + guardWeight && canGuard)
            {
                // 가드
                unit.guardAction.ExecuteGuard();
                break;
            }
        }

        yield return new WaitForSeconds(actionDelay);
        Debug.Log($"[Enemy_AI] {unit.unitData.unitName} AI 턴 종료");
    }

    /// <summary>
    /// 스킬 타겟 타입에 따른 타겟 선택
    /// </summary>
    protected Unit FindTarget(SkillData skillData)
    {
        List<Unit> targets;

        // 스킬의 targetType에 따라 타겟 목록 결정
        if (skillData.targetType == targetType.Enemy || skillData.targetType == targetType.Self)
            targets = Battle_Manager.instance.enemyUnits;  // 아군 (적 유닛들)
        else
            targets = Battle_Manager.instance.playerUnits; // 적 (플레이어 유닛들)

        targets = targets.Where(u => u != null && !u.isDead).ToList();

        if (targets.Count == 0)
            return null;

        int randomIndex = Random.Range(0, targets.Count);
        return targets[randomIndex];
    }

    /// <summary>
    /// 최적의 공격 타겟 선택 (기본: 플레이어 유닛)
    /// </summary>
    protected Unit FindBestTarget()
    {
        List<Unit> playerUnits = Battle_Manager.instance.playerUnits
            .Where(u => u != null && !u.isDead)
            .ToList();

        if (playerUnits.Count == 0)
            return null;

        int randomIndex = Random.Range(0, playerUnits.Count);
        return playerUnits[randomIndex];
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
    private void HighlightTiles(List<Tile> tiles, HighlightType highlightType)
    {
        foreach (var tile in tiles)
        {
            tile.SetHighlight(highlightType);
        }
    }
    private void ClearHighlights(List<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            tile.SetHighlight(HighlightType.None);
        }
    }
}