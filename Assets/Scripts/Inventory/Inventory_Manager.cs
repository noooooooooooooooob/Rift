using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 인벤토리 시스템 매니저 (싱글톤)
/// 슬롯 관리, 드래그 앤 드롭 처리, 장비 착용/해제 로직 담당
/// </summary>
public class Inventory_Manager : MonoBehaviour
{
    public static Inventory_Manager instance;

    [Header("Canvas Reference")]
    [SerializeField] private Canvas inventoryCanvas;

    [Header("Inventory Slots (3x3)")]
    [SerializeField] private List<Inventory_Slot> inventorySlots = new List<Inventory_Slot>();

    [Header("Equipment Slots")]
    [SerializeField] private Equipment_Slot weaponSlot;
    [SerializeField] private List<Equipment_Slot> armorSlots = new List<Equipment_Slot>();

    [Header("Prefabs")]
    [SerializeField] private GameObject draggableItemPrefab;

    [Header("Current Unit")]
    private Unit currentUnit;

    [Header("Inventory Data")]
    [SerializeField] private List<EquipmentData> inventoryItems = new List<EquipmentData>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// 특정 유닛의 인벤토리 UI를 열 때 호출
    /// </summary>
    public void OpenInventory(Unit unit)
    {
        currentUnit = unit;

        // 장비 슬롯 초기화
        if (weaponSlot != null)
            weaponSlot.Initialize(unit);

        foreach (var armorSlot in armorSlots)
        {
            if (armorSlot != null)
                armorSlot.Initialize(unit);
        }

        // UI 갱신
        RefreshInventoryUI();
        RefreshEquipmentUI();

        gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
        currentUnit = null;
    }

    /// <summary>
    /// 드래그 앤 드롭 처리
    /// </summary>
    public void HandleItemDrop(Slot_Base sourceSlot, Slot_Base targetSlot, DraggableItem draggedItem)
    {
        if (sourceSlot == targetSlot)
        {
            draggedItem.ReturnToOriginalPosition();
            return;
        }

        EquipmentData draggedEquipment = draggedItem.Equipment;
        EquipmentData existingItem = targetSlot.CurrentEquipment;
        DraggableItem existingDraggable = targetSlot.DraggableItem;

        // 대상 슬롯에 이미 아이템이 있는 경우: 스왑
        if (existingItem != null)
        {
            // 스왑 가능 여부 확인
            if (!sourceSlot.CanAcceptItem(existingItem))
            {
                draggedItem.ReturnToOriginalPosition();
                return;
            }

            // 스왑 실행
            sourceSlot.SetItem(existingItem, existingDraggable);
            if (existingDraggable != null)
            {
                existingDraggable.MoveToSlot(sourceSlot);
            }
        }
        else
        {
            sourceSlot.ClearItem();
        }

        targetSlot.SetItem(draggedEquipment, draggedItem);
        draggedItem.MoveToSlot(targetSlot);
    }

    /// <summary>
    /// 인벤토리 UI 갱신
    /// </summary>
    private void RefreshInventoryUI()
    {
        ClearAllSlots(inventorySlots);

        for (int i = 0; i < inventorySlots.Count && i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i] != null)
            {
                CreateDraggableItem(inventorySlots[i], inventoryItems[i]);
            }
        }
    }

    /// <summary>
    /// 장비 슬롯 UI 갱신
    /// </summary>
    private void RefreshEquipmentUI()
    {
        if (currentUnit == null || currentUnit.effectHandler == null) return;

        // Weapon 슬롯
        if (weaponSlot != null)
        {
            ClearSlotVisual(weaponSlot);
            EquipmentData weapon = currentUnit.effectHandler.GetWeapon();
            if (weapon != null)
            {
                CreateDraggableItem(weaponSlot, weapon);
            }
        }

        // Armor 슬롯
        List<EquipmentData> armors = currentUnit.effectHandler.GetArmors();
        for (int i = 0; i < armorSlots.Count; i++)
        {
            if (armorSlots[i] != null)
            {
                ClearSlotVisual(armorSlots[i]);
                if (i < armors.Count && armors[i] != null)
                {
                    CreateDraggableItem(armorSlots[i], armors[i]);
                }
            }
        }
    }

    private void ClearAllSlots<T>(List<T> slots) where T : Slot_Base
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                ClearSlotVisual(slot);
            }
        }
    }

    private void ClearSlotVisual(Slot_Base slot)
    {
        // 기존 DraggableItem 제거
        DraggableItem existing = slot.GetComponentInChildren<DraggableItem>();
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
        slot.ClearItem();
    }

    private void CreateDraggableItem(Slot_Base slot, EquipmentData equipment)
    {
        if (equipment == null || draggableItemPrefab == null) return;

        GameObject itemObj = Instantiate(draggableItemPrefab, slot.transform);
        RectTransform rectTransform = itemObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }

        DraggableItem draggable = itemObj.GetComponent<DraggableItem>();
        if (draggable != null)
        {
            draggable.Initialize(slot, equipment, inventoryCanvas);
            slot.SetItem(equipment, draggable);
        }
    }

    /// <summary>
    /// 인벤토리에 아이템 추가
    /// </summary>
    public bool AddItem(EquipmentData equipment)
    {
        if (equipment == null) return false;

        Inventory_Slot emptySlot = FindEmptyInventorySlot();
        if (emptySlot == null) return false;

        inventoryItems.Add(equipment);
        CreateDraggableItem(emptySlot, equipment);
        return true;
    }

    /// <summary>
    /// 인벤토리에서 아이템 제거
    /// </summary>
    public bool RemoveItem(EquipmentData equipment)
    {
        if (!inventoryItems.Contains(equipment)) return false;

        inventoryItems.Remove(equipment);
        RefreshInventoryUI();
        return true;
    }

    private Inventory_Slot FindEmptyInventorySlot()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot != null && slot.CurrentEquipment == null)
                return slot;
        }
        return null;
    }
}
