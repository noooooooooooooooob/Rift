using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Amatsu_AI : Enemy_AI
{
    Animator animator;
    public int turn = 0;
    public SkillData skill1;
    public SkillData skill2;
    public SkillData skill3;
    public SkillData ultimate;
    public BuffData burn;
    List<Tile> targets = new List<Tile>();
    Vector3 approachOffset = new Vector3(2.5f, 0, 0);
    Vector3 originalPosition;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public override IEnumerator ExecuteTurn()
    {
        Debug.Log($"[Amatsu_AI] {unit.unitData.unitName} AI 턴 {turn} 시작");

        yield return new WaitForSeconds(actionDelay);

        Unit target = FindBestTarget();
        if(turn == 0)
        {
            turn++;
            targets = Method.GetAttackableTiles(unit, skill1, target.currentTile);
            HighlightTiles(targets, HighlightType.Attackable);
            yield return StartCoroutine(Skill1Action(target));
            ClearHighlightedTiles(targets);
        }
        else if(turn == 1)
        {
            turn++;
            targets = Method.GetAttackableTiles(unit, skill2, target.currentTile);
            HighlightTiles(targets, HighlightType.Attackable);
            yield return StartCoroutine(Skill2Action(target));
            ClearHighlightedTiles(targets);
        }
        else if(turn == 2)
        {
            turn++;
            targets = Method.GetAttackableTiles(unit, skill3, target.currentTile);
            HighlightTiles(targets, HighlightType.Attackable);
            yield return StartCoroutine(Skill3Action(target));
            ClearHighlightedTiles(targets);
        }
        else
        {
            turn = 0;
            targets = Method.GetAttackableTiles(unit, ultimate, target.currentTile);
            HighlightTiles(targets, HighlightType.Attackable);
            yield return StartCoroutine(UltimateAction(target));
            ClearHighlightedTiles(targets);
        }

        animator.SetTrigger("Idle");
        yield return null;
    }
    void HighlightTiles(List<Tile> tiles, HighlightType type)
    {
        foreach (var tile in tiles)
        {
            tile.SetHighlight(type);
        }
    }
    void ClearHighlightedTiles(List<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            tile.SetHighlight(HighlightType.None);
        }
    }
    IEnumerator Skill1Action(Unit target)
    {
        originalPosition = transform.position;
        transform.DOMove(target.transform.position + unit.unitOffset + approachOffset, 0.5f);
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Skill 1");
        yield return new WaitForSeconds(4.0f);
        transform.DOMove(originalPosition, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
    public void Attack(float multiplier)
    {
        foreach (var tile in targets)
        {
            if (tile.occupant != null)
            {
                Unit targetUnit = tile.occupant.GetComponent<Unit>();
                if (targetUnit != null)
                {
                    int damage = Method.CalculateDamage(unit, targetUnit, multiplier);

                    unit.effectHandler?.OnAttack(unit);

                    targetUnit.TakeDamage(damage);
                    Method.CalculateLifeSteal(unit, damage);
                    targetUnit.effectHandler.AddBuff(burn);
                    unit.effectHandler.AddBuff(burn);
                }
            }
        }
    }
    IEnumerator Skill2Action(Unit target)
    {
        originalPosition = transform.position;
        transform.DOMove(target.transform.position + unit.unitOffset + approachOffset, 0.5f);
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Skill 2");
        yield return new WaitForSeconds(4.0f);
        transform.DOMove(originalPosition, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator Skill3Action(Unit target)
    {
        originalPosition = transform.position;
        transform.DOMove(target.transform.position + unit.unitOffset + approachOffset, 0.5f);
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Skill 3");
        yield return new WaitForSeconds(4.0f);
        transform.DOMove(originalPosition, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator UltimateAction(Unit target)
    {
        yield return null;
    }
}
