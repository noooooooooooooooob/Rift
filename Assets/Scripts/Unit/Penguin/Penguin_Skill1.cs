using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Penguin_Skill1 : Unit_Skill1
{
    public override IEnumerator ExecuteSkill1(List<Tile> tiles)
    {
        if (skill1Data == null)
        {
            Debug.LogWarning("기본 공격 스킬 데이터가 할당되지 않았습니다!");
            yield break;
        }
        

        // tiles에서 targetUnits 추출
        targetUnits.Clear();
        foreach (var tile in tiles)
        {
            if (tile.occupant != null)
            {
                Unit targetUnit = tile.occupant.GetComponent<Unit>();
                if (targetUnit != null)
                    targetUnits.Add(targetUnit);
            }
        }

        if (targetUnits.Count == 0)
        {
            Debug.LogWarning("공격할 대상이 없습니다!");
            yield break;
        }

        yield return unit.unitAnimation.PlaySkill1Animation(tiles[0].transform.position, tiles[tiles.Count - 1].transform.position);
        Debug.Log($"[Penguin_Skill1] 타일 확인: {tiles[0].name}");
        Debug.Log($"[Penguin_Skill1] 타일 확인: {tiles[tiles.Count - 1].name}");
        unit.AddUP(1);
    }
    public void Skill1_Attack()
    {
        foreach (var targetUnit in targetUnits)
        {
            if (targetUnit != null)
            {
                int damage = Method.CalculateDamage(unit, targetUnit, 2.0f);
                targetUnit.TakeDamage(damage);
                Method.CalculateLifeSteal(unit, damage);
            }
        }
    }
}
