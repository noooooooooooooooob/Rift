using UnityEngine;

/// <summary>
/// HP 비율에 따라 LS(흡혈)가 변하는 동적 효과
/// HP가 낮을수록 LS 증가
/// </summary>
[CreateAssetMenu(fileName = "HPBasedLSEffect", menuName = "Scriptable Objects/Effects/HPBasedLSEffect")]
public class HPBasedLSEffect : EffectData
{
    [Header("HP Based LS Settings")]
    [Tooltip("최대 LS 보너스 (0.3 = 30%)")]
    public float maxBonus = 0.3f;
    public float savedBonus = 0f;

    /// <summary>
    /// 현재 HP 비율에 따른 LS 보너스 계산
    /// HP 100% → 0%, HP 0% → maxBonus
    /// </summary>
    public float GetCurrentBonus(Unit unit)
    {
        if (unit == null || unit.maxHP <= 0) return 0f;

        float hpRatio = (float)unit.currentHP / unit.maxHP;
        return (1f - hpRatio) * maxBonus;
    }

    public override void Apply(Unit source, Unit target = null)
    {
        if (source == null) return;

        // 이전 보너스 빼기
        source.stats.LS -= savedBonus;

        // 새 보너스 계산 및 적용
        savedBonus = GetCurrentBonus(source);
        source.stats.LS += savedBonus;
    }

    public override void Remove(Unit source)
    {
        if (source == null) return;

        source.stats.LS -= savedBonus;
        savedBonus = 0f;
    }
}
