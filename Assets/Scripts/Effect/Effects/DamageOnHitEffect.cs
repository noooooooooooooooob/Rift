using UnityEngine;

/// <summary>
/// 추가 피해 이펙트
/// 공격 적중 시 추가 피해
/// Trigger: OnHit
/// </summary>
[CreateAssetMenu(fileName = "DamageOnHitEffect", menuName = "Scriptable Objects/Effects/Damage On Hit")]
public class DamageOnHitEffect : EffectData
{
    public enum DamageType
    {
        Flat,               // 고정 피해
        PercentOfATK,       // 공격력의 %
        PercentOfTargetMaxHP // 대상 최대 HP의 %
    }

    [Header("Damage Settings")]
    [SerializeField] private DamageType damageType;
    [SerializeField] private float value;

    public override void Apply(Unit source, Unit target = null)
    {
        if (source == null || target == null || target.isDead) return;

        int damage = damageType switch
        {
            DamageType.Flat => (int)value,
            DamageType.PercentOfATK => Mathf.RoundToInt(source.stats.ATK * value),
            DamageType.PercentOfTargetMaxHP => Mathf.RoundToInt(target.maxHP * value),
            _ => 0
        };

        if (damage > 0)
        {
            target.TakeDamage(damage);
        }
    }

    public override void Remove(Unit source) { }
}
