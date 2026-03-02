using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Serian_Skill2 : Unit_Skill2
{
    public BuffData frostbite;
    public Animator animator;
    public override IEnumerator ExecuteAttack(List<Tile> tiles)
    {
        if (skill2Data == null)
        {
            Debug.LogWarning("스킬 2 스킬 데이터가 할당되지 않았습니다!");
            yield break;
        }
        if(!Battle_Manager.instance.UsePlayerActivePoint(skill2Data.cost))
        {
            Debug.LogWarning("활성 포인트가 부족하여 공격할 수 없습니다!");
            yield break;
        }
        unit.unitAnimation.isAttacking = true;

        animator.SetTrigger("Skill 1");
        
        unit.unitAnimation.PlayVFX("Fx_IceFlake Variant", tiles[0].transform.position);

        yield return new WaitForSeconds(0.3f);
        Audio_Manager.Instance.PlaySound("serian_skill2");
        yield return new WaitForSeconds(1.4f);
        Skill2_Attack(tiles);
        unit.unitAnimation.ShakeCamera();
        yield return new WaitForSeconds(0.4f);
        animator.SetTrigger("Idle");

        foreach (var tile in tiles)
        {
            if (tile.occupant != null)
            {
                Unit targetUnit = tile.occupant.GetComponent<Unit>();
                if (targetUnit != null)
                {
                    targetUnit.effectHandler.AddBuff(frostbite);
                }
            }
        }

        yield return new WaitForSeconds(1.0f);
        unit.unitAnimation.isAttacking = false;
        unit.AddUP(1);
        yield return null;
    }
    void Skill2_Attack(List<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            Unit targetUnit = null;
            
            if (tile.occupant != null)
                targetUnit = tile.occupant.GetComponent<Unit>();
            
            if (targetUnit != null)
            {
                int damage = Method.CalculateDamage(unit, targetUnit, 2.0f);
                targetUnit.TakeDamage(damage);

                // OnHit 트리거
                unit.effectHandler?.OnHit(targetUnit, damage);

                unit.Heal(Method.CalculateLifeSteal(unit, damage));
            }
        }
    }
}
