using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HongYeon_Skill1 : Unit_Skill1
{
    public BuffData berserk;
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

        unit.unitAnimation.PlaySkill1Animation(targetPosition: tiles[0].transform.position, lastPosition: tiles[tiles.Count - 1].transform.position);
        foreach (var tile in tiles)
        {
            if (tile.occupant != null)
            {
                Unit targetUnit = tile.occupant.GetComponent<Unit>();
                targetUnit.effectHandler.AddBuff(berserk);
            }
        }
        unit.AddUP(1);
        yield return null;
    }
}
