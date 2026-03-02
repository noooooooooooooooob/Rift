using UnityEngine;

public class HongYeon_Skill2 : Unit_Skill2
{
    public BuffData bleedBuffData;
    public override void Skill2_Attack(float multiplier)
    {
        foreach (var targetUnit in targetUnits)
        {
            if (targetUnit != null)
            {
                int damage = Method.CalculateDamage(unit, targetUnit, multiplier);
                targetUnit.TakeDamage(damage);
                unit.Heal(Method.CalculateLifeSteal(unit, damage));
                targetUnit.effectHandler?.AddBuff(bleedBuffData);
            }
        }
    }
}
