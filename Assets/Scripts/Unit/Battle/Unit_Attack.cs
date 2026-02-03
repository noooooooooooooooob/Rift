using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Attack : MonoBehaviour
{
    public Unit unit;
    public SkillData attackSkillData; // 기본 공격 스킬 데이터
    
    List<Unit> targetUnits = new List<Unit>();

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }

    public virtual IEnumerator ExecuteAttack(List<Tile> tiles)
    {
        if (attackSkillData == null)
        {
            Debug.LogWarning("기본 공격 스킬 데이터가 할당되지 않았습니다!");
            yield break;
        }
        if(!Battle_Manager.instance.UsePlayerActivePoint(attackSkillData.cost))
        {
            Debug.LogWarning("활성 포인트가 부족하여 공격할 수 없습니다!");
            yield break;
        }
    
        targetUnits.Clear();
        foreach (var tile in tiles)
        {
            if (tile.occupant != null)
            {
                targetUnits.Add(tile.occupant.GetComponent<Unit>());
            }
        }

        yield return StartCoroutine(unit.unitAnimation.PlayAttackAnimation(tiles[0].transform));
        unit.AddUP(1);
    }
    public void Attack()
    {
        foreach (var targetUnit in targetUnits)
        {
            if (targetUnit != null)
            {
                int damage = Method.CalculateDamage(unit, targetUnit, 1.0f);
                targetUnit.TakeDamage(damage);
                unit.Heal(damage / 10);
                Method.CalculateLifeSteal(unit, damage);
            }
        }
    }
}
