using UnityEngine;
using System.Collections.Generic;

public enum BuffType
{
    Battle,     // 전투 중에만 (전투 끝나면 제거)
    Stage,      // 스테이지 동안 (여러 전투 유지)
    Permanent   // 영구 (유닛 고유 패시브)
}

/// <summary>
/// 버프/디버프 데이터
/// 지속 시간과 여러 이펙트를 가짐
/// </summary>
[CreateAssetMenu(fileName = "BuffData", menuName = "Scriptable Objects/BuffData")]
public class BuffData : ScriptableObject
{
    [Header("Basic Info")]
    public string buffName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Buff Settings")]
    public BuffType buffType = BuffType.Battle;  // 버프 지속 타입
    public int duration;            // 지속 턴 수 (0 = 영구)
    public int maxStacks = 1;       // 최대 중첩 수

    [Header("Effects")]
    public List<EffectData> effects = new List<EffectData>();

    /// <summary>
    /// 버프의 패시브 이펙트 적용
    /// </summary>
    public void ApplyPassiveEffects(Unit unit, int stacks = 1)
    {
        for (int i = 0; i < stacks; i++)
        {
            foreach (var effect in effects)
            {
                if (effect != null && effect.Trigger.HasFlag(EffectTrigger.Passive))
                {
                    effect.Apply(unit);
                }
            }
        }
    }

    /// <summary>
    /// 버프의 패시브 이펙트 제거
    /// </summary>
    public void RemovePassiveEffects(Unit unit, int stacks = 1)
    {
        for (int i = 0; i < stacks; i++)
        {
            foreach (var effect in effects)
            {
                if (effect != null && effect.Trigger.HasFlag(EffectTrigger.Passive))
                {
                    effect.Remove(unit);
                }
            }
        }
    }

    /// <summary>
    /// 특정 트리거의 이펙트 실행
    /// </summary>
    public void TriggerEffects(EffectTrigger trigger, Unit source, Unit target = null)
    {
        foreach (var effect in effects)
        {
            if (effect != null && effect.Trigger.HasFlag(trigger))
            {
                effect.Apply(source, target);
            }
        }
    }
}
