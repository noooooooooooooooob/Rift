using UnityEngine;

public enum EffectTrigger
{
    Passive,        // Always active while equipped
    OnAttack,       // When this unit attacks
    OnHit,          // When attack connects
    OnDamaged,      // When taking damage
    OnTurnStart,    // At start of unit's turn
    OnTurnEnd,      // At end of unit's turn
    OnKill          // When defeating an enemy
}

public interface IEffect
{
    EffectTrigger Trigger { get; }
    void Apply(Unit source, Unit target = null);
    void Remove(Unit source);
}
