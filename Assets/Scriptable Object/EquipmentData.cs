using UnityEngine;
using System.Collections.Generic;

public enum EquipmentSlot
{
    Weapon,
    Armor
}

/// <summary>
/// 장비 데이터
/// 여러 이펙트를 가질 수 있음
/// </summary>
[CreateAssetMenu(fileName = "EquipmentData", menuName = "Scriptable Objects/EquipmentData")]
public class EquipmentData : ScriptableObject
{
    [Header("Basic Info")]
    public string equipmentName;
    [TextArea] public string description;
    public EquipmentSlot slot;
    public Sprite icon;
    public int price;

    [Header("Effects")]
    public List<EffectData> effects = new List<EffectData>();

    /// <summary>
    /// 장비의 모든 이펙트 적용
    /// </summary>
    public void ApplyEffects(Unit unit)
    {
        foreach (var effect in effects)
        {
            if (effect != null && effect.Trigger.HasFlag(EffectTrigger.Passive))
            {
                effect.Apply(unit);
            }
        }
    }

    /// <summary>
    /// 장비의 모든 이펙트 제거
    /// </summary>
    public void RemoveEffects(Unit unit)
    {
        foreach (var effect in effects)
        {
            if (effect != null && effect.Trigger.HasFlag(EffectTrigger.Passive))
            {
                effect.Remove(unit);
            }
        }
    }

    /// <summary>
    /// 특정 트리거의 이펙트 가져오기
    /// </summary>
    public List<EffectData> GetEffectsByTrigger(EffectTrigger trigger)
    {
        return effects.FindAll(e => e != null && e.Trigger.HasFlag(trigger));
    }
}
