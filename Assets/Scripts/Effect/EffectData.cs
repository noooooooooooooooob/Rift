using UnityEngine;

/// <summary>
/// 이펙트의 기본 ScriptableObject 클래스
/// 모든 구체적인 이펙트는 이 클래스를 상속받아 구현
/// </summary>
public abstract class EffectData : ScriptableObject, IEffect
{
    [Header("Effect Info")]
    [SerializeField] protected string effectName;
    [SerializeField, TextArea] protected string description;
    [SerializeField] protected EffectTrigger trigger;
    [SerializeField] protected Sprite icon;

    public string EffectName => effectName;
    public string Description => description;
    public EffectTrigger Trigger => trigger;
    public Sprite Icon => icon;

    public abstract void Apply(Unit source, Unit target = null);
    public abstract void Remove(Unit source);
}
