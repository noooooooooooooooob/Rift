using UnityEngine;

/// <summary>
/// 장비 슬롯 - 특정 타입(Weapon/Armor)만 장착 가능
/// Unit의 EffectHandler와 연동하여 효과 적용/제거
/// </summary>
public class Equipment_Slot : Slot_Base
{
    [Header("Equipment Slot Settings")]
    [SerializeField] private EquipmentSlot slotType;

    public EquipmentSlot SlotType => slotType;

    private Unit targetUnit;

    public void Initialize(Unit unit)
    {
        targetUnit = unit;
    }

    public override bool CanAcceptItem(EquipmentData equipment)
    {
        if (equipment == null) return false;

        // 슬롯 타입이 일치해야 함
        if (equipment.slot != slotType) return false;

        // Armor 슬롯의 경우 최대 개수 체크
        if (slotType == EquipmentSlot.Armor && targetUnit != null)
        {
            // 이 슬롯이 비어있거나, 교체하는 경우는 허용
            if (CurrentEquipment != null) return true;
            return targetUnit.effectHandler.CanEquipArmor();
        }

        return true;
    }

    public override void SetItem(EquipmentData equipment, DraggableItem item = null)
    {
        // 기존 장비 효과 제거
        if (CurrentEquipment != null && targetUnit != null)
        {
            if (slotType == EquipmentSlot.Weapon)
                targetUnit.effectHandler.UnequipSlot(EquipmentSlot.Weapon);
            else
                targetUnit.effectHandler.UnequipArmor(CurrentEquipment);
        }

        base.SetItem(equipment, item);

        // 새 장비 효과 적용
        if (equipment != null && targetUnit != null)
        {
            targetUnit.effectHandler.EquipItem(equipment);
        }
    }

    public override void ClearItem()
    {
        // 장비 효과 제거
        if (CurrentEquipment != null && targetUnit != null)
        {
            if (slotType == EquipmentSlot.Weapon)
                targetUnit.effectHandler.UnequipSlot(EquipmentSlot.Weapon);
            else
                targetUnit.effectHandler.UnequipArmor(CurrentEquipment);
        }

        base.ClearItem();
    }
}
