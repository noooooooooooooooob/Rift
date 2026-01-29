using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 공용 인벤토리 슬롯
/// 비어있을 때만 아이템 수용, 이미 아이템이 있으면 드롭 거부
/// </summary>
public class Inventory_Slot : SlotBase
{
    [Header("Inventory Slot")]
    public int slotIndex;

    /// <summary>
    /// 아이템 수용 가능 여부 - 비어있을 때만 가능
    /// </summary>
    public override bool CanAcceptItem(EquipmentData item)
    {
        return IsEmpty && item != null;
    }

    /// <summary>
    /// 아이템 드롭 처리
    /// </summary>
    public override void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggable = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggable == null) return;

        EquipmentData droppedItem = draggable.Item;
        if (droppedItem == null) return;

        // 비어있을 때만 수용
        if (!CanAcceptItem(droppedItem))
        {
            // 취소 - 원래 위치로 돌아감
            return;
        }

        // 출발 슬롯에서 아이템 제거
        SlotBase sourceSlot = draggable.SourceSlot;
        if (sourceSlot != null)
        {
            // 장비 슬롯에서 왔다면 장비 해제 처리
            if (sourceSlot is Equipment_Slot equipSlot)
            {
                equipSlot.UnequipItem();
            }
            else
            {
                sourceSlot.ClearItem();
            }
        }

        // 인벤토리 매니저 데이터 업데이트
        Inventory_Manager.instance.SetItem(slotIndex, droppedItem);

        // 이 슬롯에 아이템 설정
        SetItem(droppedItem);

        // 드래그 성공 처리
        draggable.OnDropSuccess();

        // 새 DraggableItem 생성
        Inventory_Manager.instance.CreateDraggableForSlot(this);
    }

    /// <summary>
    /// 슬롯에 아이템 설정 (Inventory_Manager 동기화)
    /// </summary>
    public override void SetItem(EquipmentData item)
    {
        base.SetItem(item);
        Inventory_Manager.instance.SetItem(slotIndex, item);
    }

    /// <summary>
    /// 슬롯에서 아이템 제거 (Inventory_Manager 동기화)
    /// </summary>
    public override EquipmentData ClearItem()
    {
        EquipmentData removed = base.ClearItem();
        Inventory_Manager.instance.SetItem(slotIndex, null);
        return removed;
    }

    /// <summary>
    /// Inventory_Manager의 데이터로 슬롯 갱신
    /// </summary>
    public void RefreshFromManager()
    {
        currentItem = Inventory_Manager.instance.GetItem(slotIndex);
        UpdateVisual();
    }
}
