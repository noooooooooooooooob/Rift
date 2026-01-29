using UnityEngine;

/// <summary>
/// 인벤토리 시스템 테스트용 스크립트
/// 게임 시작 시 테스트 아이템을 인벤토리에 추가
/// </summary>
public class InventoryTest : MonoBehaviour
{
    [Header("Test Items")]
    [Tooltip("테스트용 장비들을 여기에 할당")]
    public EquipmentData[] testItems;

    [Header("Settings")]
    public bool addItemsOnStart = true;

    void Start()
    {
        if (addItemsOnStart)
        {
            AddTestItems();
        }
    }

    /// <summary>
    /// 테스트 아이템 추가
    /// </summary>
    [ContextMenu("Add Test Items")]
    public void AddTestItems()
    {
        if (Inventory_Manager.instance == null)
        {
            Debug.LogError("Inventory_Manager.instance is null!");
            return;
        }

        foreach (var item in testItems)
        {
            if (item != null)
            {
                int index = Inventory_Manager.instance.AddItem(item);
                if (index >= 0)
                {
                    Debug.Log($"[InventoryTest] Added {item.equipmentName} to slot {index}");
                }
            }
        }

        // UI 갱신
        if (Inventory_Manager.instance.IsOpen)
        {
            Inventory_Manager.instance.RefreshInventory();
        }
    }

    /// <summary>
    /// 인벤토리 비우기
    /// </summary>
    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        if (Inventory_Manager.instance == null) return;

        Inventory_Manager.instance.ClearInventory();
        Debug.Log("[InventoryTest] Inventory cleared");

        if (Inventory_Manager.instance.IsOpen)
        {
            Inventory_Manager.instance.RefreshInventory();
        }
    }

    /// <summary>
    /// 인벤토리 열기
    /// </summary>
    [ContextMenu("Open Inventory")]
    public void OpenInventory()
    {
        if (Inventory_Manager.instance == null) return;
        Inventory_Manager.instance.Open();
    }

    /// <summary>
    /// 인벤토리 닫기
    /// </summary>
    [ContextMenu("Close Inventory")]
    public void CloseInventory()
    {
        if (Inventory_Manager.instance == null) return;
        Inventory_Manager.instance.Close();
    }
}
