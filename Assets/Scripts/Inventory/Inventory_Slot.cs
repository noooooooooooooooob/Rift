using UnityEngine;

/// <summary>
/// 인벤토리 슬롯 - 모든 종류의 장비를 보관 가능
/// </summary>
public class Inventory_Slot : Slot_Base
{
    public override bool CanAcceptItem(EquipmentData equipment)
    {
        return equipment != null;
    }
}
