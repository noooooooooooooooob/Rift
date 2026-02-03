using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 유닛의 이펙트를 관리하는 컴포넌트
/// 장비 이펙트와 버프를 처리
/// </summary>
public class EffectHandler : MonoBehaviour
{
    private Unit unit;
    public Buff_Container_UI buffContainerUI;

    [Header("Equipment")]
    [SerializeField] private EquipmentData weapon;
    [SerializeField] private List<EquipmentData> armors = new List<EquipmentData>();
    private const int MAX_ARMOR = 3;

    [Header("Active Buffs")]
    [SerializeField] private List<BuffInstance> activeBuffs = new List<BuffInstance>();

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    #region Equipment

    public void EquipItem(EquipmentData equipment)
    {
        if (equipment == null || unit == null) return;

        switch (equipment.slot)
        {
            case EquipmentSlot.Weapon:
                UnequipSlot(EquipmentSlot.Weapon);
                weapon = equipment;
                break;
            case EquipmentSlot.Armor:
                if (armors.Count >= MAX_ARMOR) return; // 최대 3개
                armors.Add(equipment);
                break;
        }

        equipment.ApplyEffects(unit);
    }

    public void UnequipSlot(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon:
                if (weapon != null)
                {
                    weapon.RemoveEffects(unit);
                    weapon = null;
                }
                break;
            case EquipmentSlot.Armor:
                foreach (var armor in armors)
                {
                    armor.RemoveEffects(unit);
                }
                armors.Clear();
                break;
        }
    }

    public void UnequipArmor(EquipmentData armor)
    {
        if (armor == null || !armors.Contains(armor)) return;
        armor.RemoveEffects(unit);
        armors.Remove(armor);
    }

    public EquipmentData GetWeapon() => weapon;

    public List<EquipmentData> GetArmors() => armors;

    public bool CanEquipArmor() => armors.Count < MAX_ARMOR;

    #endregion

    #region Buffs

    public void AddBuff(BuffData buffData)
    {
        if (buffData == null || unit == null) return;

        // 기존 버프 확인
        var existing = activeBuffs.Find(b => b.buffData == buffData);

        if (existing != null)
        {
            if (existing.AddStack())
            {
                // 스택 추가 시 패시브 이펙트 추가 적용
                buffData.ApplyPassiveEffects(unit, 1);
            }
            existing.RefreshDuration();
        }
        else
        {
            // 새 버프 추가
            var instance = new BuffInstance(buffData);
            activeBuffs.Add(instance);
            buffData.ApplyPassiveEffects(unit, 1);
        }

        buffContainerUI?.RefreshBuffs();
    }

    public void RemoveBuff(BuffData buffData)
    {
        var instance = activeBuffs.Find(b => b.buffData == buffData);
        if (instance != null)
        {
            buffData.RemovePassiveEffects(unit, instance.stacks);
            activeBuffs.Remove(instance);
            buffContainerUI?.RefreshBuffs();
        }
    }

    public void ClearAllBuffs()
    {
        foreach (var buff in activeBuffs)
        {
            buff.buffData.RemovePassiveEffects(unit, buff.stacks);
        }
        activeBuffs.Clear();
        buffContainerUI?.ClearAll();
    }

    public bool HasBuff(BuffData buffData)
    {
        return activeBuffs.Exists(b => b.buffData == buffData);
    }

    public List<BuffInstance> GetActiveBuffs() => activeBuffs;

    #endregion

    #region Trigger Events

    /// <summary>
    /// 턴 시작 시 호출
    /// </summary>
    public void OnTurnStart()
    {
        TriggerEffects(EffectTrigger.OnTurnStart);
    }

    /// <summary>
    /// 턴 종료 시 호출
    /// </summary>
    public void OnTurnEnd()
    {
        TriggerEffects(EffectTrigger.OnTurnEnd);
        TickBuffDurations();
    }

    /// <summary>
    /// 공격 시 호출
    /// </summary>
    public void OnAttack(Unit target)
    {
        TriggerEffects(EffectTrigger.OnAttack, target);
    }

    /// <summary>
    /// 공격 적중 시 호출
    /// </summary>
    public void OnHit(Unit target, int damage)
    {
        TriggerEffects(EffectTrigger.OnHit, target);
        TriggerLifesteal(damage);
    }

    /// <summary>
    /// 피해를 받을 때 호출
    /// </summary>
    public void OnDamaged(Unit attacker)
    {
        TriggerEffects(EffectTrigger.OnDamaged, attacker);
    }

    /// <summary>
    /// 적 처치 시 호출
    /// </summary>
    public void OnKill(Unit target)
    {
        TriggerEffects(EffectTrigger.OnKill, target);
    }

    private void TriggerEffects(EffectTrigger trigger, Unit target = null)
    {
        // 장비 이펙트
        TriggerEquipmentEffects(trigger, target);

        // 버프 이펙트
        foreach (var buff in activeBuffs)
        {
            buff.buffData.TriggerEffects(trigger, unit, target);
        }
    }

    private void TriggerEquipmentEffects(EffectTrigger trigger, Unit target)
    {
        // Weapon
        if (weapon != null)
        {
            foreach (var effect in weapon.GetEffectsByTrigger(trigger))
            {
                effect.Apply(unit, target);
            }
        }

        // Armors
        foreach (var armor in armors)
        {
            if (armor == null) continue;
            foreach (var effect in armor.GetEffectsByTrigger(trigger))
            {
                effect.Apply(unit, target);
            }
        }
    }

    private void TriggerLifesteal(int damage)
    {
        // Weapon
        if (weapon != null)
        {
            foreach (var effect in weapon.effects)
            {
                if (effect is LifestealEffect lifesteal)
                {
                    lifesteal.ApplyWithDamage(unit, damage);
                }
            }
        }

        // Armors
        foreach (var armor in armors)
        {
            if (armor == null) continue;
            foreach (var effect in armor.effects)
            {
                if (effect is LifestealEffect lifesteal)
                {
                    lifesteal.ApplyWithDamage(unit, damage);
                }
            }
        }

        // Buffs
        foreach (var buff in activeBuffs)
        {
            foreach (var effect in buff.buffData.effects)
            {
                if (effect is LifestealEffect lifesteal)
                {
                    lifesteal.ApplyWithDamage(unit, damage);
                }
            }
        }
    }

    private void TickBuffDurations()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (activeBuffs[i].TickDuration())
            {
                activeBuffs[i].buffData.RemovePassiveEffects(unit, activeBuffs[i].stacks);
                activeBuffs.RemoveAt(i);
            }
        }
        buffContainerUI?.RefreshBuffs();
    }

    #endregion
}
