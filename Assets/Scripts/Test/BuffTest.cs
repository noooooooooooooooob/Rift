using UnityEngine;

public class BuffTest : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Unit targetUnit;

    [Header("Buff to Add")]
    [SerializeField] private BuffData buffData;

    [ContextMenu("Add Buff")]
    public void AddBuff()
    {
        if (targetUnit == null || buffData == null)
        {
            Debug.LogWarning("Target Unit 또는 BuffData가 없습니다.");
            return;
        }

        var effectHandler = targetUnit.GetComponent<EffectHandler>();
        if (effectHandler != null)
        {
            effectHandler.AddBuff(buffData);
            Debug.Log($"{targetUnit.unitData.unitName}에게 {buffData.buffName} 버프 추가!");
        }
    }

    [ContextMenu("Remove Buff")]
    public void RemoveBuff()
    {
        if (targetUnit == null || buffData == null) return;

        var effectHandler = targetUnit.GetComponent<EffectHandler>();
        if (effectHandler != null)
        {
            effectHandler.RemoveBuff(buffData);
            Debug.Log($"{targetUnit.unitData.unitName}에게서 {buffData.buffName} 버프 제거!");
        }
    }

    [ContextMenu("Clear All Buffs")]
    public void ClearAllBuffs()
    {
        if (targetUnit == null) return;

        var effectHandler = targetUnit.GetComponent<EffectHandler>();
        if (effectHandler != null)
        {
            effectHandler.ClearAllBuffs();
            Debug.Log($"{targetUnit.unitData.unitName}의 모든 버프 제거!");
        }
    }
}
