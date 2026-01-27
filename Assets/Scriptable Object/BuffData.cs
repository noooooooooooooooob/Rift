using UnityEngine;
using System.Collections.Generic;

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
                if (effect != null && effect.Trigger == EffectTrigger.Passive)
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
                if (effect != null && effect.Trigger == EffectTrigger.Passive)
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
            if (effect != null && effect.Trigger == trigger)
            {
                effect.Apply(source, target);
            }
        }
    }
}
