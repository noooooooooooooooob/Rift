using UnityEngine;

public class Penguin_Attack : Unit_Attack
{
    public override void Attack()
    {
        foreach (var targetUnit in targetUnits)
        {
            if (targetUnit != null)
            {
                int damage = Method.CalculateDamage(unit, targetUnit, 1f);
                targetUnit.TakeDamage(damage);
                Method.CalculateLifeSteal(unit, damage);
            }
        }
    }
}
