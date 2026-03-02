using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit_Skill1 : MonoBehaviour
{
    public Unit unit;
    public SkillData skill1Data; // 기본 공격 스킬 데이터

    public List<Unit> targetUnits = new List<Unit>();

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }

    public virtual IEnumerator ExecuteSkill1(List<Tile> tiles)
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
                if (targetUnit != null)
                {
                    targetUnit.TakeDamage(100);
                }
            }
        }
        unit.AddUP(1);
        yield return null;
    }
}
