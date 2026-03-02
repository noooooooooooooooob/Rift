using UnityEngine;

/// <summary>
/// 출혈 효과 - 턴 종료 시 대미지
/// Trigger: OnTurnEnd
/// </summary>
[CreateAssetMenu(fileName = "BleedEffect", menuName = "Scriptable Objects/Effects/BleedEffect")]
public class BleedEffect : EffectData
{
    public override void Apply(Unit source, Unit target = null)
    {
        Debug.Log($"[BleedEffect] Apply 호출됨! source: {source?.unitData?.unitName}");

        if (source == null || source.effectHandler == null) return;

        // 스택 수 가져오기
        int stacks = GetStacks(source);

        int damage =  stacks * 10;

        if (damage > 0)
        {
            source.TakeDamage(damage, DamageTextType.Normal);
            Debug.Log($"[Bleed] {source.unitData.unitName}이(가) 출혈({stacks}스택)로 {damage} 대미지!");
        }
    }

    private int GetStacks(Unit unit)
    {
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
        // 출혈은 Remove 시 특별한 처리 없음
    }
}
