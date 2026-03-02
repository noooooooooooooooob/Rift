using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Serian_Attack : Unit_Attack
{
    public Animator animator;
    public override IEnumerator ExecuteAttack(List<Tile> tiles)
    {
        if (tiles == null || tiles.Count == 0)
        {
            Debug.LogWarning("공격할 타일이 없습니다!");
            yield break;
        }
        if (attackSkillData == null)
        {
            Debug.LogWarning("기본 공격 스킬 데이터가 할당되지 않았습니다!");
            yield break;
        }
        unit.unitAnimation.isAttacking = true;
    
        targetUnits.Clear();
        foreach (var tile in tiles)
        {
            if (tile.occupant != null)
            {
                targetUnits.Add(tile.occupant.GetComponent<Unit>());
            }
        }

        animator.SetTrigger("Attack");
        unit.unitAnimation.PlayVFX("ManaSplash Variant", tiles[0].transform.position);
        Attack();

        yield return new WaitForSeconds(1.0f);
        animator.SetTrigger("Idle");
        unit.unitAnimation.isAttacking = false;
        unit.AddUP(1);
    }
    
    public virtual void Attack()
    {
        foreach (var targetUnit in targetUnits)
        {
            if (targetUnit != null)
            {
                int damage = Method.CalculateDamage(unit, targetUnit, 1.0f);
                targetUnit.TakeDamage(damage);

                // OnHit 트리거
                unit.effectHandler?.OnHit(targetUnit, damage);

                unit.Heal(Method.CalculateLifeSteal(unit, damage));
            }
        }
    }
}
