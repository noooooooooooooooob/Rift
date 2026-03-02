using UnityEngine;
using System;

[Flags]
public enum EffectTrigger
{
    None = 0,
    Passive = 1,        // Always active while equipped
    OnAttack = 2,       // When this unit attacks
    OnHit = 4,          // When attack connects
    OnDamaged = 8,      // When taking damage
    OnTurnStart = 16,   // At start of unit's turn
    OnTurnEnd = 32,     // At end of unit's turn
    OnKill = 64         // When defeating an enemy
}

public interface IEffect
{
    EffectTrigger Trigger { get; }
    void Apply(Unit source, Unit target = null);
    void Remove(Unit source);
}
