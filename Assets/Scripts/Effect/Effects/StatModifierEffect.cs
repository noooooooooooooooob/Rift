using UnityEngine;

/// <summary>
/// 스탯 수정 이펙트
/// 특정 스탯에 고정값 또는 비율 보너스를 적용
/// Trigger: Passive
/// </summary>
[CreateAssetMenu(fileName = "StatModifierEffect", menuName = "Scriptable Objects/Effects/Stat Modifier")]
public class StatModifierEffect : EffectData
{
    public enum ModifierType
    {
        Flat,       // 고정값 (ATK + 10)
        Percent     // 비율 (ATK * 1.1)
    }

    [Header("Stat Modifier Settings")]
    [SerializeField] private StatType statType;
    [SerializeField] private ModifierType modifierType;
    [SerializeField] private float value;

    public override void Apply(Unit source, Unit target = null)
    {
        if (source == null || source.stats == null) return;
        ApplyModifier(source.stats, value);
        UpdateMaxHP(source);
    }

    public override void Remove(Unit source)
    {
        if (source == null || source.stats == null) return;
        ApplyModifier(source.stats, -value);
        UpdateMaxHP(source);
    }

    private void ApplyModifier(UnitStat stats, float mod)
    {
        switch (statType)
        {
            case StatType.HP:
                stats.HP += modifierType == ModifierType.Flat ? (int)mod : (int)(stats.HP * mod);
                break;
            case StatType.UP:
                stats.UP += modifierType == ModifierType.Flat ? (int)mod : (int)(stats.UP * mod);
                break;
            case StatType.ATK:
                stats.ATK += modifierType == ModifierType.Flat ? (int)mod : (int)(stats.ATK * mod);
                break;
            case StatType.DEF:
                stats.DEF += modifierType == ModifierType.Flat ? (int)mod : (int)(stats.DEF * mod);
                break;
            case StatType.SPD:
                stats.SPD += modifierType == ModifierType.Flat ? (int)mod : (int)(stats.SPD * mod);
                break;
            case StatType.AGI:
                stats.AGI += modifierType == ModifierType.Flat ? (int)mod : (int)(stats.AGI * mod);
                break;
            case StatType.LS:
                stats.LS = Mathf.Clamp01(stats.LS + (modifierType == ModifierType.Flat ? mod : stats.LS * mod));
                break;
            case StatType.RST:
                stats.RST = Mathf.Clamp01(stats.RST + (modifierType == ModifierType.Flat ? mod : stats.RST * mod));
                break;
            case StatType.EV:
                stats.EV = Mathf.Clamp01(stats.EV + (modifierType == ModifierType.Flat ? mod : stats.EV * mod));
                break;
            case StatType.CA:
                stats.CA = Mathf.Clamp01(stats.CA + (modifierType == ModifierType.Flat ? mod : stats.CA * mod));
                break;
            case StatType.DR:
                stats.DR = Mathf.Clamp(stats.DR + (modifierType == ModifierType.Flat ? mod : stats.DR * mod), 0f, 0.99f);
                break;
            case StatType.AP:
                stats.AP += modifierType == ModifierType.Flat ? mod : stats.AP * mod;
                break;
        }
    }

    private void UpdateMaxHP(Unit source)
    {
        if (statType == StatType.HP)
        {
            source.maxHP = source.stats.HP;
            if (source.currentHP > source.maxHP)
                source.currentHP = source.maxHP;
        }
    }
}
