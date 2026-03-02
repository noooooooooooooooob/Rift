using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Serian_Skill1 : Unit_Skill1
{
    public Animator animator;
    public BuffData burn;
    public override IEnumerator ExecuteSkill1(List<Tile> tiles)
    {
        if (skill1Data == null)
        {
            Debug.LogWarning("기본 공격 스킬 데이터가 할당되지 않았습니다!");
            yield break;
        }
        if(!Battle_Manager.instance.UsePlayerActivePoint(skill1Data.cost))
        {
            Debug.LogWarning("활성 포인트가 부족하여 공격할 수 없습니다!");
            yield break;
        }
        unit.unitAnimation.isAttacking = true;

        animator.SetTrigger("Skill 2");

        Vector3 targetPosition = tiles[0].transform.position;
        
        unit.unitAnimation.PlayVFX("Explosion", targetPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)));
        Audio_Manager.Instance.PlaySound("serian_skill1");
        unit.unitAnimation.ShakeCamera();
        Skill1_Attack(tiles);
        yield return new WaitForSeconds(0.2f);
        
        unit.unitAnimation.PlayVFX("Explosion", targetPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)));
        Audio_Manager.Instance.PlaySound("serian_skill1");
        unit.unitAnimation.ShakeCamera();
        Skill1_Attack(tiles);
        yield return new WaitForSeconds(0.2f);
        
        unit.unitAnimation.PlayVFX("Explosion", targetPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)));
        Audio_Manager.Instance.PlaySound("serian_skill1");
        unit.unitAnimation.ShakeCamera();
        Skill1_Attack(tiles);
        yield return new WaitForSeconds(0.2f);
        
        unit.unitAnimation.PlayVFX("Explosion", targetPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)));
        Audio_Manager.Instance.PlaySound("serian_skill1");
        unit.unitAnimation.ShakeCamera();
        Skill1_Attack(tiles);
        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(1.5f);

        yield return null;
        foreach (var tile in tiles)
        {
            if (tile.occupant != null)
            {
                Unit targetUnit = tile.occupant.GetComponent<Unit>();
                if (targetUnit != null)
                {
                    targetUnit.effectHandler.AddBuff(burn);
                }
            }
        }
        unit.AddUP(1);
        unit.unitAnimation.isAttacking = false;
        
        yield return null;
    }
    void Skill1_Attack(List<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            Unit targetUnit = null;
            
            if (tile.occupant != null)
                targetUnit = tile.occupant.GetComponent<Unit>();
            
            if (targetUnit != null)
            {
                int damage = Method.CalculateDamage(unit, targetUnit, 0.2f);
                targetUnit.TakeDamage(damage);

                // OnHit 트리거
                unit.effectHandler?.OnHit(targetUnit, damage);

                unit.Heal(Method.CalculateLifeSteal(unit, damage));
            }
        }
    }
}

