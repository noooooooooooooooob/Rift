using UnityEngine;

/// <summary>
/// 화상 효과 - 턴 종료 시 현재 체력 비례 대미지
/// Trigger: OnTurnEnd
/// </summary>
[CreateAssetMenu(fileName = "BurnEffect", menuName = "Scriptable Objects/Effects/Burn Effect")]
public class BurnEffect : EffectData
{
    [Header("Burn Settings")]
    [SerializeField] private float damagePercent = 0.05f;

    [SerializeField] private float percentPerStack = 0.02f;

    [Tooltip("최소 대미지")]
    [SerializeField] private int minDamage = 1;

    public override void Apply(Unit source, Unit target = null)
    {
        if (source == null) return;

        int stacks = GetStacks(source);
        float totalPercent = damagePercent + (percentPerStack * (stacks - 1));

        // 현재 체력 비례 대미지 계산
        int damage = Mathf.Max(minDamage, Mathf.RoundToInt(source.currentHP * totalPercent));

        if (damage > 0)
        {
            source.TakeDamage(damage, DamageTextType.Normal);
            Debug.Log($"[Burn] {source.unitData.unitName}이(가) 화상({stacks}스택, {totalPercent * 100}%)으로 {damage} 대미지!");
        }
    }

    private int GetStacks(Unit unit)
    {
        if (unit.effectHandler == null) return 1;

        foreach (var buff in unit.effectHandler.GetActiveBuffs())
        {
            if (buff.buffData.effects.Contains(this))
            {
                return buff.stacks;
            }
        }
        return 1;
    }

    public override void Remove(Unit source)
    {
        // 화상은 Remove 시 특별한 처리 없음
    }
}
