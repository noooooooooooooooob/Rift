using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slime_Attack : Unit_Attack
{
    public override IEnumerator ExecuteAttack(List<Tile> tiles)
    {
        if (attackSkillData == null)
        {
            Debug.LogWarning("기본 공격 스킬 데이터가 할당되지 않았습니다!");
            yield break;
        }
        Unit targetUnit = null;
        targetUnits = Battle_Manager.instance.enemyUnits;

        targetUnit = targetUnits[Random.Range(0, targetUnits.Count)];
        targetUnit.currentTile.SetHighlight(HighlightType.Attackable);
        if(targetUnit != null)
        {
            targetUnit.Heal((int)((float)unit.stats.ATK * 1.0f));
            VFX_Manager.Instance.Play("Healing", targetUnit.transform.position);
            yield return new WaitForSeconds(2.0f);
        }
        targetUnit.currentTile.SetHighlight(HighlightType.None);
        yield return null;
    }
}
