using UnityEngine;

/// <summary>
/// 턴 회복 이펙트
/// 턴 시작 또는 종료 시 회복
/// Trigger: OnTurnStart 또는 OnTurnEnd
/// </summary>
[CreateAssetMenu(fileName = "HealOnTurnEffect", menuName = "Scriptable Objects/Effects/Heal On Turn")]
public class HealOnTurnEffect : EffectData
{
    public enum HealType
    {
        Flat,           // 고정 회복량
        PercentOfMaxHP  // 최대 HP의 %
    }

    [Header("Heal Settings")]
    [SerializeField] private HealType healType;
    [SerializeField] private float value;

    public override void Apply(Unit source, Unit target = null)
    {
        if (source == null || source.isDead) return;

        int healAmount = healType == HealType.Flat
            ? (int)value
            : Mathf.RoundToInt(source.maxHP * value);

        if (healAmount > 0)
        {
            source.Heal(healAmount);
        }
    }

    public override void Remove(Unit source) { }
}
