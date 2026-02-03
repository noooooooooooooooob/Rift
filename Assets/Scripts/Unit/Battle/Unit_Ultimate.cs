using UnityEngine;
using System.Collections.Generic;

public class Unit_Ultimate : MonoBehaviour
{
    public Unit unit;
    public SkillData ultimateSkillData; // 궁극기 스킬 데이터
    List<Unit> targetUnits = new List<Unit>();

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }

    /// <summary>
    /// 궁극기 실행
    /// </summary>
    public virtual void ExecuteUltimate(List<Tile> tiles)
    {
        if (ultimateSkillData == null)
        {
            Debug.LogWarning("궁극기 스킬 데이터가 할당되지 않았습니다!");
            return;
        }

        // UP 소모
        unit.currentUP = 0;
        unit.upBar.UpdateBar(0, unit.stats.UP);
        unit.actionMenuUI.ultimateButton.interactable = false;

        targetUnits.Clear();
        foreach (var tile in tiles)
        {
            if (tile.occupant != null)
            {
                targetUnits.Add(tile.occupant.GetComponent<Unit>());
            }
        }

        // 궁극기 애니메이션 재생
        unit.unitAnimation.PlayUltimateAnimation();

        // 대상에게 데미지
        foreach (var tile in tiles)
        {
            if (tile.occupant != null)
            {
                Unit targetUnit = tile.occupant.GetComponent<Unit>();
                if (targetUnit != null)
                {
                    targetUnit.TakeDamage(1);
                }
            }
        }

        Debug.Log($"[Ultimate] {unit.unitData.unitName} 궁극기 사용!");
    }

    public virtual void Ultimate_Attack(float multiplier)
    {
        foreach (var targetUnit in targetUnits)
        {
            if (targetUnit != null)
            {
                int damage = Method.CalculateDamage(unit, targetUnit, multiplier);
                targetUnit.TakeDamage(damage);
                Method.CalculateLifeSteal(unit, damage);
            }
        }
    }
}
