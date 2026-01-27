using UnityEngine;

/// <summary>
/// 생명력 흡수 이펙트
/// 공격 적중 시 피해량의 일정 비율만큼 회복
/// Trigger: OnHit
/// </summary>
[CreateAssetMenu(fileName = "LifestealEffect", menuName = "Scriptable Objects/Effects/Lifesteal")]
public class LifestealEffect : EffectData
{
    [Header("Lifesteal Settings")]
    [SerializeField, Range(0f, 1f)] private float lifestealPercent = 0.1f;

    // damage 정보가 필요하므로 ApplyWithDamage 사용 권장
    public override void Apply(Unit source, Unit target = null)
    {
        // 기본 구현 - damage 정보 없이는 동작하지 않음
    }

    /// <summary>
    /// 피해량 기반 회복
    /// </summary>
    public void ApplyWithDamage(Unit source, int damage)
    {
        if (source == null || source.isDead) return;

        int healAmount = Mathf.RoundToInt(damage * lifestealPercent);
        if (healAmount > 0)
        {
            source.Heal(healAmount);
        }
    }

    public override void Remove(Unit source) { }
}
