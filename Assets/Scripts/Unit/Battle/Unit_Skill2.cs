using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Skill2 : MonoBehaviour
{
    public Unit unit;
    public SkillData skill2Data;

    List<Unit> targetUnits = new List<Unit>();

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }
    public virtual IEnumerator ExecuteAttack(List<Tile> tiles)
    {
        if (skill2Data == null)
        {
            Debug.LogWarning("스킬 데이터가 할당되지 않았습니다!");
            yield break;
        }
        if(!Battle_Manager.instance.UsePlayerActivePoint(skill2Data.cost))
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

        yield return StartCoroutine(unit.unitAnimation.PlaySkill2Animation(tiles[0].transform));
        unit.AddUP(1);
    }
    public void Skill2_Attack(float multiplier)
    {
        foreach (var targetUnit in targetUnits)
        {
            if (targetUnit != null)
            {
                int damage = Method.CalculateDamage(unit, targetUnit, multiplier);
                targetUnit.TakeDamage(damage);
                unit.Heal(damage / 10);
                Method.CalculateLifeSteal(unit, damage);
            }
        }
    }
}
