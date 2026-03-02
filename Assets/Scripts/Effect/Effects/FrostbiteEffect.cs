using UnityEngine;

/// <summary>
/// 동상 효과 - 스택당 SPD 감소
/// Trigger: Passive
/// </summary>
[CreateAssetMenu(fileName = "FrostbiteEffect", menuName = "Scriptable Objects/Effects/FrostbiteEffect")]
public class FrostbiteEffect : EffectData
{
    [Header("Frostbite Settings")]
    [Tooltip("스택당 SPD 감소 비율 (0.1 = 10%)")]
    [SerializeField] private float spdReductionPerStack = 0.1f;

    public override void Apply(Unit source, Unit target = null)
    {
        if (source == null || source.stats == null || source.unitData == null) return;

        // 기본 SPD 기준으로 스택당 감소량 계산
        int reduction = Mathf.RoundToInt(source.unitData.baseStat.SPD * spdReductionPerStack);
        source.stats.SPD -= reduction;

        // SPD가 0 이하로 내려가지 않도록
        if (source.stats.SPD < 1)
            source.stats.SPD = 1;

        Debug.Log($"[Frostbite] {source.unitData.unitName}의 SPD {reduction} 감소! (현재 SPD: {source.stats.SPD})");
    }

    public override void Remove(Unit source)
    {
        if (source == null || source.stats == null || source.unitData == null) return;

        // 기본 SPD 기준으로 스택당 감소량 복원
        int reduction = Mathf.RoundToInt(source.unitData.baseStat.SPD * spdReductionPerStack);
        source.stats.SPD += reduction;

        Debug.Log($"[Frostbite] {source.unitData.unitName}의 SPD {reduction} 복원! (현재 SPD: {source.stats.SPD})");
    }
}
