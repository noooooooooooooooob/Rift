using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 유닛 장비 슬롯
/// 아이템 타입 검사 (무기/방어구)
/// 이미 장비가 있으면 스왑
/// </summary>
public class Equipment_Slot : SlotBase
{
    [Header("Equipment Slot")]
    public EquipmentSlot slotType;
    public int armorIndex; // 방어구 슬롯인 경우 0-2

    private UnitEquipment targetEquipment;

    /// <summary>
    /// 대상 유닛 장비 설정
    /// </summary>
    public void SetTargetEquipment(UnitEquipment equipment)
    {
        targetEquipment = equipment;
        RefreshFromEquipment();
    }

    /// <summary>
    /// 아이템 수용 가능 여부 - 타입이 맞아야 하고, 씬 제한 통과해야 함
    /// </summary>
    public override bool CanAcceptItem(EquipmentData item)
    {
        if (item == null) return false;
        if (targetEquipment == null) return false;
        if (!Inventory_Manager.CanModifyEquipment) return false; // 씬 제한
        return item.slot == slotType;
    }

    /// <summary>
    /// 아이템 드롭 처리
    /// </summary>
    public override void OnDrop(PointerEventData eventData)
    {
        // 씬 제한 체크
        if (!Inventory_Manager.CanModifyEquipment) return;

        DraggableItem draggable = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggable == null) return;

        EquipmentData droppedItem = draggable.Item;
        if (droppedItem == null) return;

        // 타입 검사
        if (!CanAcceptItem(droppedItem))
        {
            // 취소 - 원래 위치로 돌아감
            return;
        }

        SlotBase sourceSlot = draggable.SourceSlot;

        // 이미 장비가 있으면 스왑
        if (!IsEmpty)
        {
            EquipmentData oldItem = currentItem;

            // 출발 슬롯 처리
            if (sourceSlot != null)
            {
                if (sourceSlot is Equipment_Slot sourceEquipSlot)
                {
                    // 장비슬롯 → 장비슬롯: 스왑
                    sourceEquipSlot.UnequipItem();
                    sourceEquipSlot.EquipItem(oldItem);
                    Inventory_Manager.instance.CreateDraggableForSlot(sourceEquipSlot);
                }
                else if (sourceSlot is Inventory_Slot sourceInvSlot)
                {
                    // 인벤토리 → 장비슬롯 (장비 있음): 스왑
                    sourceInvSlot.ClearItem();
                    sourceInvSlot.SetItem(oldItem);
                    Inventory_Manager.instance.CreateDraggableForSlot(sourceInvSlot);
                }
            }

            // 새 장비 장착
            EquipItem(droppedItem);
        }
        else
        {
            // 빈 슬롯: 단순 장착
            if (sourceSlot != null)
            {
                if (sourceSlot is Equipment_Slot sourceEquipSlot)
                {
                    sourceEquipSlot.UnequipItem();
                }
                else
                {
                    sourceSlot.ClearItem();
                }
            }

            EquipItem(droppedItem);
        }

        // 드래그 성공 처리
        draggable.OnDropSuccess();

        // 새 DraggableItem 생성
        Inventory_Manager.instance.CreateDraggableForSlot(this);
    }

    /// <summary>
    /// 장비 장착 (UnitEquipment에 효과 적용)
    /// </summary>
    public void EquipItem(EquipmentData item)
    {
        if (item == null) return;
        if (targetEquipment == null) return;

        // UnitEquipment를 통해 장착 (효과 적용됨)
        if (slotType == EquipmentSlot.Weapon)
        {
            targetEquipment.weapon = item;
            item.ApplyEffects(targetEquipment.GetComponent<Unit>());
        }
        else
        {
            if (targetEquipment.armors == null || armorIndex < 0 || armorIndex >= targetEquipment.armors.Length)
            {
                Debug.LogError($"Invalid armor index: {armorIndex}, armors length: {targetEquipment.armors?.Length ?? 0}");
                return;
            }
            targetEquipment.armors[armorIndex] = item;
            item.ApplyEffects(targetEquipment.GetComponent<Unit>());
        }

        currentItem = item;
        UpdateVisual();
    }

    /// <summary>
    /// 장비 해제 (UnitEquipment에서 효과 제거)
    /// </summary>
    public EquipmentData UnequipItem()
    {
        if (currentItem == null) return null;
        if (targetEquipment == null) return null;

        EquipmentData removed = currentItem;
        Unit unit = targetEquipment.GetComponent<Unit>();

        // 효과 제거
        removed.RemoveEffects(unit);

        // UnitEquipment에서 제거
        if (slotType == EquipmentSlot.Weapon)
        {
            targetEquipment.weapon = null;
        }
        else
        {
            if (targetEquipment.armors != null && armorIndex >= 0 && armorIndex < targetEquipment.armors.Length)
            {
                targetEquipment.armors[armorIndex] = null;
            }
        }

        currentItem = null;
        UpdateVisual();

        return removed;
    }

    /// <summary>
    /// UnitEquipment의 데이터로 슬롯 갱신
    /// </summary>
    public void RefreshFromEquipment()
    {
        if (targetEquipment == null)
        {
            currentItem = null;
        }
        else
        {
            currentItem = targetEquipment.GetEquipment(slotType, armorIndex);
        }
        UpdateVisual();
    }

    /// <summary>
    /// 슬롯 초기화 (유닛 선택 해제 시)
    /// </summary>
    public void ClearSlot()
    {
        targetEquipment = null;
        currentItem = null;
        UpdateVisual();
    }
}
